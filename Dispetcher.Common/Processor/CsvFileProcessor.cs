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

        public CsvFileProcessor()
        {
            _thread = new Thread(QueueThread);
            _thread.Name = "QueueThread";
            _thread.IsBackground = true;
            _thread.Start();
        }

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
                        ProcessCSV(filename);
                        File.Delete(filename);
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

        /// <summary>
        /// Неподсредственно обработка CSV файла
        /// </summary>
        /// <param name="filename"></param>
        private void ProcessCSV(string filename)
        {
            var items = new List<CsvItem>();
            
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

                        CsvItem csvItem;
                        try
                        {
                            csvItem = ConvertToCsvItem(line);
                            items.Add(csvItem);
                        }
                        catch (Exception ex)
                        {
                            // TODO log строка i
                        }
                    }
                }
            }

            // TODO тут должна быть какая-то валидация items на повторяющиеся записи

            var dbManager = Locator.Resolve<IDbManager>();
            var transaction = dbManager.BeginTransaction();
            try
            {
                foreach (var csvItem in items)
                {
                    dbManager.ExecNonQuery("INSERT INTO [Journal] (Date, SideNumber, Schedule, Route, VehicleType) " +
                                           "VALUES (@date, @sidenumber, @schedule, @route, @vehicletype)",
                        new Dictionary<string, object>()
                        {
                            { "date", csvItem.Date }, 
                            { "sidenumber", csvItem.SideNumber },
                            { "schedule", csvItem.Schedule },
                            { "route", csvItem.RouteName },
                            { "vehicletype", csvItem.VehicleType }
                        });
                }
                transaction.Commit();
            }
            catch(Exception ex)
            {
                // TODO log
                transaction.Rollback();
            }

        }

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

            csvItem.SideNumber = items[1];
            csvItem.RouteName = items[2];
            csvItem.Schedule = items[3];
            
            VehicleType vt;
            if (!Enum.TryParse<VehicleType>(items[4], true, out vt))
                throw new Exception("Не удалось разобрать тип транспорта.");

            csvItem.VehicleType = vt;

            return csvItem;
        }
    }
}
