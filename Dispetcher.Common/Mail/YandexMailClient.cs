using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Dispetcher.Common.Events;
using S22.Imap;

namespace Dispetcher.Common.Mail
{
    public class YandexMailClient : IMailClient
    {
        private const string serverAddress = "imap.yandex.ru";
        private const int serverPort = 993;

        private readonly string _username;
        private readonly string _password;
        private readonly string _attachmentTempFolder;

        public YandexMailClient(string username, string password, string attachmentTempFolder)
        {
            this._username = username;
            this._password = password;
            this._attachmentTempFolder = attachmentTempFolder;
        }

        /// <summary>
        /// Событие при получении новго файла
        /// </summary>
        public event AttachmentSavedEventHandler AttachmentSaved;

        public string SaveFolder { get { return _attachmentTempFolder; } }

        private readonly object _checkSyncLock = new object();

        /// <summary>
        /// Проверить почту
        /// </summary>
        public void Check()
        {
            lock (_checkSyncLock)
            {
                using (var client = new ImapClient(serverAddress, serverPort, _username, _password, AuthMethod.Login, true))
                {
                    var uids = client.Search(SearchCondition.Unseen()).ToList();
                    foreach (var uid in uids)
                    {
                        try
                        {
                            var mailMessage = client.GetMessage(uid, false);
                            var files = ExtractAttachments(uid, mailMessage).ToList();
                            client.SetMessageFlags(uid, null, MessageFlag.Seen);

                            if (AttachmentSaved != null)
                            {
                                foreach (var tmpFilename in files)
                                {
                                    AttachmentSaved(mailMessage.Date(), tmpFilename);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            // TODO log
                        }
                    }
                }
            }
        }

        private IEnumerable<string> ExtractAttachments(uint uid, MailMessage message)
        {
            var i = 0;
            foreach (var att in message.Attachments)
            {
                i++;

                var tmpFilename = Path.Combine(_attachmentTempFolder, String.Format("{0}_{1}.csv", uid, i));

                using (Stream file = File.Create(tmpFilename))
                {
                    att.ContentStream.CopyTo(file);
                }

                yield return tmpFilename;
            }            
        }
    }
}
