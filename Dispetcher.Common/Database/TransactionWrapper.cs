using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispetcher.Common.IoC;

namespace Dispetcher.Common.Database
{
    public class TransactionWrapper
    {
        private static Lazy<IDbManager> dbManagerLazy = new Lazy<IDbManager>(() => Locator.Resolve<IDbManager>());
        private static IDbManager DbManager { get { return dbManagerLazy.Value; } }

        /// <summary>
        /// Выполняет действие с БД в транзакции.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onError"></param>
        public static void Execute(Action action, Action<Exception> onError = null)
        {
            DbTransaction transaction = DbManager.BeginTransaction();

            try
            {
                action();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                if (onError != null)
                {
                    onError(ex);
                }
            }
        }
    }
}
