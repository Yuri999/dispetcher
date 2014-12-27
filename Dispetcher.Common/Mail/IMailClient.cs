using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Dispetcher.Common.Events;

namespace Dispetcher.Common.Mail
{
    /// <summary>
    /// Работа с почтой
    /// </summary>
    public interface IMailClient
    {
        /// <summary>
        /// Событие при сохранении на диск вложения
        /// </summary>
        event AttachmentSavedEventHandler AttachmentSaved;

        /// <summary>
        /// Проверяет новую почту и сохраняет все вложения на диск
        /// </summary>
        void Check();

        string SaveFolder { get; }
    }
}
