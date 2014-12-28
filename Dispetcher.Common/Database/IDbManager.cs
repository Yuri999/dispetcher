using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data;
using Dispetcher.Common.IoC;

namespace Dispetcher.Common.Database
{
    [ComponentInterface(LifeTime = ComponentLifeTime.Singleton, AllowMultiple = false)]
    public interface IDbManager
    {
        event EventHandler OnConnect;
        event EventHandler<ConnectErrorEventArgs> OnConnectError;
        event EventHandler<StateChangeEventArgs> OnConnectionStateChange;

        void Connect();
        
        [Obsolete]
        void ConnectAsync();
        
        void Disconnect();

        IEnumerable<T> ExecQuery<T>(string sqlQuery, Dictionary<string, object> parameters = null);

        int ExecNonQuery(string sqlQuery, Dictionary<string, object> parameters = null);

        bool Connected { get; }

        DbTransaction BeginTransaction();
    }
}
