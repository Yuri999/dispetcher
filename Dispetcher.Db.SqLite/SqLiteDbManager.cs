using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Configuration;
using System.Data;
using System.Threading;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Database;
using System.Data.SQLite;

namespace Dispetcher.Db.SqLite
{
    
    [Component(Order = 20)]
    public class SqLiteDbManager : IDbManager
    {
        private SQLiteConnection connection;

        private readonly object connectingSyncObj = new object();
        private bool connecting;
        private DateTime? lastAttempt;
        private int attemptCount;

        public bool Connected
        {
            get
            {
                var conn = connection;
                if (conn == null)
                    return false;

                lock (conn)
                {
                    return conn.State == ConnectionState.Open || conn.State == ConnectionState.Fetching || conn.State == ConnectionState.Executing;
                }
            }
        }

        public DbTransaction BeginTransaction()
        {
            return connection.BeginTransaction();
        }

        public event EventHandler OnConnect;
        public event EventHandler<ConnectErrorEventArgs> OnConnectError;

        public event EventHandler<StateChangeEventArgs> OnConnectionStateChange;

        public void Connect()
        {
            ConnectTask();
            
            if (!Connected)
                throw new Exception("Не удалось подключиться к БД.");
        }

        [Obsolete]
        public void ConnectAsync()
        {
            lock (connectingSyncObj)
            {
                if (!connecting)
                {
                    var thread = new Thread(ConnectTask);
                    thread.Start();
                }
            }
        }

        private void ConnectTask()
        {
            lock (connectingSyncObj)
            {
                connecting = true;
            }

            var setting = ConfigurationManager.ConnectionStrings["SqLiteConnection"];
            if (setting == null)
            {
                if (OnConnectError != null)
                {
                    OnConnectError(this, new ConnectErrorEventArgs() { Exception = new Exception("SqLiteConnection string is not set") });
                }

                return;
            }

            var connectionString = setting.ConnectionString;


            var maxAttempts = 10;
            var connectInterval = 20;

            attemptCount = 0;
            lastAttempt = null;

            while (!Connected && attemptCount < maxAttempts)
            {
                if (lastAttempt == null || DateTime.Now - lastAttempt > new TimeSpan(0, 0, connectInterval))
                {
                    try
                    {
                        lastAttempt = DateTime.Now;
                        attemptCount++;

                        connection = new SQLiteConnection(connectionString);
                        connection.StateChange += ConnectionOnStateChange;
                        connection.Open();

                        if (OnConnect != null)
                        {
                            OnConnect(this, EventArgs.Empty);
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (OnConnectError != null)
                        {
                            OnConnectError(this, new ConnectErrorEventArgs() { Exception = ex });
                        }
                    }
                }

                Thread.Sleep(100);
            }

            if (!Connected)
            {
                if (OnConnectError != null)
                {
                    OnConnectError(this, new ConnectErrorEventArgs() { Attempt = attemptCount, Exception = new AttemptCountExceededException() });
                }
            }

            lock (connectingSyncObj)
            {
                connecting = false;
            }
        }

        private void ConnectionOnStateChange(object sender, StateChangeEventArgs stateChangeEventArgs)
        {
            if (OnConnectionStateChange != null)
            {
                OnConnectionStateChange(sender, stateChangeEventArgs);
            }
        }

        public IEnumerable<T> ExecQuery<T>(string sqlQuery)
        {
            var type = typeof(T);
            var constructor = typeof(T).GetConstructor(new Type[0]);
            if (constructor == null)
                throw new Exception(String.Format("Нет конструктора по умолчанию для типа {0}.", type.ToString()));

            var cmd = connection.CreateCommand();
            cmd.CommandText = sqlQuery;
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var values = reader.GetValues();
                    var item = (T)constructor.Invoke(null);
                    foreach (var key in values.AllKeys)
                    {
                        var prop = type.GetProperty(key);
                        if (prop.CanWrite)
                        {
                            prop.SetValue(item, values[key]);
                        }
                    }

                    yield return item;
                }
            }
        }

        public int ExecNonQuery(string sqlQuery, Dictionary<string,object> parameters = null)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sqlQuery;

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = item.Key;
                    p.Value = item.Value;
                    cmd.Parameters.Add(p);
                }
            }

            return cmd.ExecuteNonQuery();
        }


        public void Disconnect()
        {
            try
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
