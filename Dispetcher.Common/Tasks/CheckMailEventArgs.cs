using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispetcher.Common.Mail;

namespace Dispetcher.Common.Tasks
{
    public class CheckMailEventArgs : EventArgs
    {
        public CheckMailEventArgs(CheckMailResult checkResult)
        {
            CheckResult = checkResult;
        }
        
        public CheckMailResult CheckResult { get; set; }
    }
}
