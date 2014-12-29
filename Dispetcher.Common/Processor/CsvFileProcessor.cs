using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Dispetcher.Common.Database;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Mail;
using Dispetcher.Common.Models;

namespace Dispetcher.Common.Processor
{
    public class CsvFileProcessor
    {
        private const string csvDateFormat1 = "dd.MM.yyyy";
        private const string csvDateFormat2 = "dd.MM.yy";
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);
        
        private IMailClient _mailClient;

        private Thread _thread;
        private bool _terminated;

        private readonly Queue<string> _files = new Queue<string>(); 

        private Lazy<IDbManager> dbManagerLazy = new Lazy<IDbManager>(() => Locator.Resolve<IDbManager>());
        private IDbManager dbManager { get { return dbManagerLazy.Value; } }

        public CsvFileProcessor()
        {
            _thread = new Thread(QueueThread);
            _thread.Name = "QueueThread";
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        /// Действие перед записью новых данных в БД
        /// </summary>
        public event Action BeforeDataChange;

        /// <summary>
        /// Действие после записи новых данных в БД
        /// </summary>
        public event Action AfterDataChange;
        
        private void QueueThread()
        {
            while (!_terminated)
            {
                string filename = null;

                lock (_files)
                {
                    if (_files.Count > 0)
                    {
                        filename = _files.Dequeue();
                    }
                }

                if (filename != null)
                {
                    try
                    {
                        ProcessFile(filename);
                    }
                    catch (Exception ex)
                    {
                        // TODO log
                    }
                }

                Thread.Sleep(100);
            }
        }

        public void Subscribe(IMailClient mailClient)
        {
            _mailClient = mailClient;
            _mailClient.AttachmentSaved += MailClientOnAttachmentSaved;
        }

        public void Unsubscribe()
        {
            if (_mailClient != null)
            {
                _mailClient.AttachmentSaved -= MailClientOnAttachmentSaved;
            }
        }

        public void Dispose()
        {
            _terminated = true;
            _thread = null;
            Unsubscribe();
        }

        public void CheckExistingFiles(string folder)
        {
            var files = Directory.GetFiles(folder);
            lock (_files)
            {
                foreach (var fileName in files)
                {
                    _files.Enqueue(fileName);
                }
            }
        }

        private void MailClientOnAttachmentSaved(DateTime? messageDate, string attachmentFilename)
        {
            lock (_files)
            {
                _files.Enqueue(attachmentFilename);
            }
        }

        private void ProcessFile(string filename)
        {
            var newItems = ReadItems(filename, true).ToList();

            if (BeforeDataChange != null)
            {
                BeforeDataChange();
            }

            try
            {
                var transaction = dbManager.BeginTransaction();
                try
                {
                    var dates = newItems.Select(x => x.Date).Distinct().ToArray();

                    // считываем из базы защищенные записи
                    var protectedItems = dbManager.ExecQuery<CsvItem>("SELECT * FROM [Journal] WHERE Date IN (@dates) AND Protected = 1",
                        new Dictionary<string, object>() {{"dates", dates}}).ToList();

                    // удалям все незащищенные записи за эти числа
                    dbManager.ExecNonQuery("DELETE FROM [Journal] WHERE Date IN (@dates) AND Protected = 0",
                        new Dictionary<string, object>() { { "dates", dates } });

                    // TODO как мержить изменения то ???

                    InsertItems(newItems);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // TODO log
                    transaction.Rollback();
                }
                
                File.Delete(filename);
            }
            finally
            {
                if (AfterDataChange != null)
                {
                    AfterDataChange();
                }
            }            
        }

        /// <summary>
        /// Парсинг строк CSV файла
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="skipWrongLines">Пропускать строки с ошибками или выбрасывать исключение.</param>
        private List<CsvItem> ReadItems(string filename, bool skipWrongLines)
        {
            var dict = new Dictionary<string, CsvItem>();

            using (var fileStream = System.IO.File.OpenRead(filename))
            {
                using (var reader = new StreamReader(fileStream, _encoding))
                {
                    var i = 0;
                    while (!reader.EndOfStream)
                    {
                        i++;
                        var line = reader.ReadLine();
                        if (String.IsNullOrEmpty(line))
                            continue;

                        CsvItem csvItem = null;
                        try
                        {
                            csvItem = ConvertToCsvItem(line);
                        }
                        catch (Exception ex)
                        {
                            // TODO log строка i
                            if (!skipWrongLines)
                                throw;
                        }

                        if (csvItem != null)
                        {
                            var key = String.Format("{0}|{1}|{2}|{3}", csvItem.Date.ToShortDateString(), csvItem.Route, csvItem.VehicleType, csvItem.Schedule);
                            if (!dict.ContainsKey(key))
                            {
                                dict.Add(key, csvItem);
                            }
                            else
                            {
                                dict[key] = csvItem;
                            }
                        }
                    }
                }
            }

            return dict.Values.ToList();
        }

        private void InsertItems(IEnumerable<CsvItem> items)
        {
            foreach (var csvItem in items)
            {
                dbManager.ExecNonQuery("INSERT INTO [Journal] (Date, SideNumberPlan, SideNumberFact, Schedule, Route, VehicleType, Protected) " +
                                        "VALUES (@date, @sidenumberplan, @sidenumberfact, @schedule, @route, @vehicletype, @protected)",
                    new Dictionary<string, object>()
                    {
                        { "date", csvItem.Date }, 
                        { "sidenumberplan", csvItem.SideNumberPlan },
                        { "sidenumberfact", csvItem.SideNumberFact },
                        { "schedule", csvItem.Schedule },
                        { "route", csvItem.Route },
                        { "vehicletype", csvItem.VehicleType },
                        { "protected", false },
                    });
            }
        }

        private void DeleteItems(IEnumerable<CsvItem> items)
        {
            dbManager.ExecNonQuery("DELETE FROM [Journal] WHERE Id IN (@ids)",
                new Dictionary<string, object>() { { "ids", items.Select(i => i.Id).ToArray() } });
        }

        /// <summary>
        /// Парсит строку в объект CsvItem
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private CsvItem ConvertToCsvItem(string line)
        {
            var items = line.Split(new [] { ';' }).ToList();
            if (items.Count < 5)
                return null;

            var csvItem = new CsvItem();

            DateTime date;
            if (DateTime.TryParseExact(items[0], csvDateFormat1, null, DateTimeStyles.None, out date))
            {
                csvItem.Date = date;
            }
            else
            {
                if (DateTime.TryParseExact(items[0], csvDateFormat2, null, DateTimeStyles.None, out date))
                {
                    csvItem.Date = date;
                }
                else
                {
                    throw new Exception("Не удалось разобрать дату.");
                }
            }

            csvItem.SideNumberPlan = items[1];
            csvItem.SideNumberFact = items[1];
            csvItem.Schedule = items[2];
            csvItem.Route = items[3];
            
            VehicleType vt;
            if (!Enum.TryParse<VehicleType>(items[4], true, out vt))
                throw new Exception("Не удалось разобрать тип транспорта.");

            csvItem.VehicleType = vt;

            return csvItem;
        }

        class DateSideNummberEqualityComparer : IEqualityComparer<CsvItem>
        {
            public bool Equals(CsvItem x, CsvItem y)
            {
                return x.DateSideNumberHash == y.DateSideNumberHash;
            }

            public int GetHashCode(CsvItem obj)
            {
                return obj.DateSideNumberHash;
            }
        }
    }
}
