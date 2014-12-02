using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispetcher.Common.Database
{
    public class ConnectErrorEventArgs : EventArgs
    {
        public int Attempt { get; set; }
        public Exception Exception { get; set; }
    }
}
