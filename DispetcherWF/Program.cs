using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispetcher.Common.Database;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Mail;
using Dispetcher.Common.Processor;
using Dispetcher.Common.Tasks;

namespace DispetcherWF
{
    static class Program
    {

        private static CheckMailboxTask _checkMailboxTask;
        private static CsvFileProcessor _csvFileProcessor;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += ApplicationOnApplicationExit;

            try
            {
                Init();
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+"\r\n\r\n"+ex.StackTrace, "Ошибка запуска");
                Application.Exit();
            }
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
            Application.ApplicationExit -= ApplicationOnApplicationExit;
            Dispose();
        }

        private static void Init()
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
            IocInitializer.RegisterInstance(_checkMailboxTask);
            #endregion

            #region запускаем процессор CSV
            _csvFileProcessor = new CsvFileProcessor();
            _csvFileProcessor.Subscribe(mailClient);
            _csvFileProcessor.CheckExistingFiles(mailClient.SaveFolder);
            IocInitializer.RegisterInstance(_csvFileProcessor);
            #endregion
        }

        private static void Dispose()
        {
            if (_csvFileProcessor != null)
                _csvFileProcessor.Dispose();

            if (_checkMailboxTask != null)
                _checkMailboxTask.Stop();
        }
    }
}
