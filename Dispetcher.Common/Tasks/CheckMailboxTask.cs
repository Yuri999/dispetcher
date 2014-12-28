using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Mail;

namespace Dispetcher.Common.Tasks
{
    public class CheckMailboxTask : ITask
    {
        private Thread _thread;
        private bool _terminated;
        private readonly int _checkInterval;
        private readonly IMailClient _mailClient;

        /// <summary>
        /// Задача по проверке ящика. Периодически запускает <see cref="IMailClient.Check"/>.
        /// </summary>
        /// <param name="mailClient"></param>
        /// <param name="interval">Интервал проверки в миллисекундах</param>
        public CheckMailboxTask(IMailClient mailClient, int interval)
        {
            _mailClient = mailClient;
            _checkInterval = interval;
        }

        public void Start()
        {
            _thread = new Thread(ThreadWork);
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void ThreadWork()
        {
            while (!_terminated)
            {
                try
                {
                    CheckMailbox();
                }
                catch (Exception e)
                {
                    // TODO log
                }

                Thread.Sleep(_checkInterval);
            }
        }

        private void CheckMailbox()
        {
            _mailClient.Check();
        }

        public void Stop()
        {
            if (_thread != null)
            {
                _terminated = true;
                _thread = null;
            }
        }
    }
}
