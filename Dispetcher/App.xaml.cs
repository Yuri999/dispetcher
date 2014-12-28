using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Dispetcher.Common.Database;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Mail;
using Dispetcher.Common.Processor;
using Dispetcher.Common.Tasks;

namespace Dispetcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Init();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();
            base.OnExit(e);
        }

        private CheckMailboxTask _checkMailboxTask;
        private CsvFileProcessor _csvFileProcessor;

        private void Init()
        {
            try
            {
                // инициализируем контейнер
                IocInitializer.Init();

                // соединяемся с локальной БД
                Locator.Resolve<IDbManager>().Connect();

                StructureCreator.Create();

                var mailClient = Locator.Resolve<IMailClient>();

                #region Запускаем периодический забор почты и скидывание на диск
                _checkMailboxTask = new CheckMailboxTask(mailClient, 5000);
                _checkMailboxTask.Start();
                #endregion

                #region запускаем процессор CSV
                var mailClient = Locator.Resolve<IMailClient>();
                _csvFileProcessor = new CsvFileProcessor();
                _csvFileProcessor.Subscribe(mailClient);
                _csvFileProcessor.CheckExistingFiles(mailClient.SaveFolder);
                #endregion
            }
            catch (Exception e)
            {
                Dispose();
                MessageBox.Show(e.Message, "Ошибка запуска");
                Application.Current.Shutdown();
            }
        }

        private void Dispose()
        {
            if (_csvFileProcessor != null)
                _csvFileProcessor.Dispose();

            if (_checkMailboxTask != null)
                _checkMailboxTask.Stop();
        }
    }
}
