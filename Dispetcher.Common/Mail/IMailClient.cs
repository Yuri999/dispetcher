using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Dispetcher.Common.Events;
using Dispetcher.Common.IoC;

namespace Dispetcher.Common.Mail
{
    /// <summary>
    /// Работа с почтой
    /// </summary>
    [ComponentInterface(AllowMultiple = false, LifeTime = ComponentLifeTime.Singleton)]
    public interface IMailClient
    {
        /// <summary>
        /// Событие при сохранении на диск вложения
        /// </summary>
        event AttachmentSavedEventHandler AttachmentSaved;

        /// <summary>
        /// Проверяет новую почту и сохраняет все вложения на диск
        /// </summary>
        CheckMailResult Check();

        string SaveFolder { get; }
    }
}
