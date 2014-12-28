using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Mail;
using Dispetcher.Common.Tasks;

namespace Dispetcher.Common.Processor
{
    public class CsvFileProcessor
    {
        private IMailClient _mailClient;

        public CsvFileProcessor()
        {
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
            Unsubscribe();
        }

        public void CheckExistingFiles(string folder)
        {
            var files = Directory.GetFiles(folder);
            foreach (var fileName in files)
            {
                Enqueue(fileName);
            }
        }

        private void MailClientOnAttachmentSaved(DateTime? messageDate, string attachmentFilename)
        {
            Enqueue(attachmentFilename);
        }

        private void Enqueue(string fileName)
        {
            
        }
    }
}
