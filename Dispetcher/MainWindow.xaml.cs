using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Dispetcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IocInitializer.Init();
            DbManager.OnConnectError += InstanceOnConnectError;
            DbManager.OnConnectionStateChange += InstanceOnConnectionStateChange;
        }

        private IDbManager DbManager {get { return Locator.Resolve<IDbManager>(); }}

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            DbManager.ConnectAsync();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            DbManager.Disconnect();
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
                    ? new SolidColorBrush(Color.FromRgb(50, 255, 50)) 
                    : null;
            });
        }
    }
}
