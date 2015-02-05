using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispetcher.Common.Mail
{
    public class CheckMailResult
    {
        public CheckMailResult()
        {
            Exceptions = new List<Exception>();
        }

        public int NewMessagesCount { get; set; }
        
        public int ProcessedMessagesCount { get; set; }

        public List<Exception> Exceptions { get; set; }
    }
}
