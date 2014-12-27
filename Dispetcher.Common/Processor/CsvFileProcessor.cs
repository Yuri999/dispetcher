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
    class CsvFileProcessor
    {
        private IMailClient _mailClient;

        public CsvFileProcessor()
        {
            _mailClient = Locator.Resolve<IMailClient>();
            _mailClient.AttachmentSaved += MailClientOnAttachmentSaved;

            CheckExistingFiles();
        }

        private void CheckExistingFiles()
        {
            var files = Directory.GetFiles(_mailClient.SaveFolder);
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
