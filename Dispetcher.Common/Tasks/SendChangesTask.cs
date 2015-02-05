using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dispetcher.Common.Database;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Models;

namespace Dispetcher.Common.Tasks
{
    public class SendChangesTask : ITask
    {
        private Lazy<IDbManager> dbManagerLazy = new Lazy<IDbManager>(() => Locator.Resolve<IDbManager>());
        private IDbManager DbManager { get { return dbManagerLazy.Value; } }
        
        private bool _terminated;
        private DateTime _lastSend = DateTime.MinValue;
        private readonly object _sendSync = new object();
        private int _sendCount = 0;

        public void Start()
        {
            (new Thread(() =>
            {
                while (!_terminated)
                {
                    if (DateTime.Now - _lastSend > new TimeSpan(0, 0, 30))
                    {
                        Interlocked.Increment(ref _sendCount);
                    }
                    
                    if (_sendCount > 0)
                    {
                        try
                        {
                            SendData();
                        }
                        catch (Exception ex)
                        {
                            // TODO log
                        }
                        _lastSend = DateTime.Now;
                        Interlocked.Decrement(ref _sendCount);
                    }

                    Thread.Sleep(100);
                }
            }) {IsBackground = true}).Start();
        }

        public void Stop()
        {
            _terminated = true;
        }

        private void SendData()
        {
            lock (_sendSync)
            {
                // до четырех утра считаем что это еще предыдущий день
                var date = DateTime.Now.Hour < 4 ? DateTime.Today.AddDays(-1) : DateTime.Today;

                var items = DbManager.ExecQuery<CsvItem>("SELECT * FROM [Journal] WHERE Date = @date",
                    new Dictionary<string, object>() { { "date", date } }).Select(x => String.Format("{0}-{1}-{2}-{3}", (int)x.VehicleType, x.Route, x.Schedule, x.SideNumberFact)).ToList();

                var postData = "Items=" + string.Join("+", items);

                WebRequest request = WebRequest.Create("http://gde-edet.com/Dispetcher/Receiver");
                
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                WebResponse response = request.GetResponse();
                var status = ((HttpWebResponse)response).StatusDescription;
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        string responseFromServer = reader.ReadToEnd();
                    }
                }
                response.Close();
            }
        }

        public void QueueSend()
        {
            Interlocked.Increment(ref _sendCount);
        }
    }
}
