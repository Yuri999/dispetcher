using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        void ConnectAsync();
        void Disconnect();

        T[] ExecQuery<T>(string sqlQuery);
        int ExecNonQuery(string sqlQuery);

        void CreateStructure();

        bool Connected { get; }
    }
}
