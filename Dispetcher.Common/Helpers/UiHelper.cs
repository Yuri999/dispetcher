using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Dispetcher.Common.Helpers
{

    public class UiHelper
    {
        /// <summary>
        /// Выполнить действие в UI потоке
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action">действие</param>
        /// <remarks>Если текущий поток это и есть UI, то действие выполняется синхронно в нем же</remarks>
        public static void RunInUiThread(Dispatcher dispatcher, Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            try
            {
                dispatcher.Invoke(action);
            }
            catch (Exception)
            {
            }
        }
    }
}
