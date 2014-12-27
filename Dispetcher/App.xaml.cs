using System;
using System.Collections.Generic;
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
                IocInitializer.Init();

                Locator.Resolve<IDbManager>().ConnectAsync();

                _checkMailboxTask = new CheckMailboxTask(5000);
                _checkMailboxTask.Start();

                _csvFileProcessor = new CsvFileProcessor();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Current.Shutdown();
            }
        }

        private void Dispose()
        {
            _csvFileProcessor.Dispose();
            _checkMailboxTask.Stop();
        }
    }
}
