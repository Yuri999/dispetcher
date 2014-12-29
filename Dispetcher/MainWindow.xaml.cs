using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Dispetcher.Common.Database;
using Dispetcher.Common.Helpers;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Models;
using Dispetcher.Common.Processor;

namespace Dispetcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Color _connectionOkColor = Color.FromRgb(50, 255, 50);

        private Lazy<CsvFileProcessor> _csvFileProcessorLazy = new Lazy<CsvFileProcessor>(() => Locator.Resolve<CsvFileProcessor>());
        private CsvFileProcessor CsvFileProcessor { get { return _csvFileProcessorLazy.Value; } }

        private Lazy<IDbManager> dbManagerLazy = new Lazy<IDbManager>(() => Locator.Resolve<IDbManager>());
        private IDbManager DbManager { get { return dbManagerLazy.Value; } }


        private ObservableCollection<CsvItem> MyList = new ObservableCollection<CsvItem>();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                DbManager.OnConnectError += InstanceOnConnectError;
                DbManager.OnConnectionStateChange += InstanceOnConnectionStateChange;

                rectInit.Fill = DbManager.Connected ? new SolidColorBrush(_connectionOkColor) : null;

                LoadData();
                CsvFileProcessor.BeforeDataChange += CsvFileProcessorOnBeforeDataChange;
                CsvFileProcessor.AfterDataChange += CsvFileProcessorOnAfterDataChange;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка запуска");
            }
        }

        private void LoadData()
        {
            MyList.Clear();

            var items = DbManager.ExecQuery<CsvItem>("SELECT * FROM [Journal] WHERE Date = @date",
                new Dictionary<string, object>() {{"date", /*DateTime.Today*/ new DateTime(2014,09,07)}}).ToList();

            foreach (var csvItem in items)
            {
                MyList.Add(csvItem);
            }
        }

        private void CsvFileProcessorOnAfterDataChange()
        {
            LoadData();
        }

        private void CsvFileProcessorOnBeforeDataChange()
        {
            
        }

        private void InstanceOnConnectError(object sender, ConnectErrorEventArgs connectErrorEventArgs)
        {
            UiHelper.RunInUiThread(Dispatcher, () =>
            {
                var s = String.Format("{0} Connection Error: {1}", DateTime.Now, connectErrorEventArgs.Exception.Message);
                lstBox.Items.Add(s);
            });
        }

        private void InstanceOnConnectionStateChange(object sender, StateChangeEventArgs stateChangeEventArgs)
        {
            UiHelper.RunInUiThread(Dispatcher, () =>
            {
                var s = String.Format("{0} {1}", DateTime.Now, stateChangeEventArgs.CurrentState);
                lstBox.Items.Add(s);
                rectInit.Fill = stateChangeEventArgs.CurrentState == ConnectionState.Open
                    ? new SolidColorBrush(_connectionOkColor)
                    : null;
            });
        }
    }
}
