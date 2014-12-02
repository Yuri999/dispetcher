using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Threading;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Database;
using System.Data.SqlClient;

namespace Dispetcher.Db.MsSql
{
    
    [Component(Order = 30)]
    public class MsSqlDbManager : IDbManager
    {
        private SqlConnection connection;

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

        public event EventHandler OnConnect;
        public event EventHandler<ConnectErrorEventArgs> OnConnectError;

        public event EventHandler<StateChangeEventArgs> OnConnectionStateChange;

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

            var setting = ConfigurationManager.ConnectionStrings["MSSQLConnection"];
            if (setting == null)
            {
                if (OnConnectError != null)
                {
                    OnConnectError(this, new ConnectErrorEventArgs() { Exception = new Exception("MSSQLConnection string is not set") });
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

                        connection = new SqlConnection(connectionString);
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

        public T[] ExecQuery<T>(string sqlQuery)
        {
            throw new NotImplementedException();
        }

        public int ExecNonQuery(string sqlQuery)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sqlQuery;
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

        public void CreateStructure()
        {
            //ExecNonQuery(Resources.database);
        }
    }
}
