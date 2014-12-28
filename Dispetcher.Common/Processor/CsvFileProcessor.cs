using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Dispetcher.Common.Mail;

namespace Dispetcher.Common.Processor
{
    public class CsvFileProcessor
    {
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
            using (var fileStream = System.IO.File.OpenRead(filename))
            {
                using (var reader = new StreamReader(fileStream, _encoding))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (String.IsNullOrEmpty(line))
                            continue;

                        var items = line.Split(new char[] { ';' }).ToList();

                        // TODO
                    }
                }
            }
        }
    }
}
