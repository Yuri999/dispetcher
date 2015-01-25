using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispetcher.Common.Models
{
    /// <summary>
    /// Запись о действиях пользователя
    /// </summary>
    public class UserLog
    {
        public long Id { get; set; }

        public DateTime Date { get; set; }

        public UserActionType ActionType { get; set; }

        public long RecId { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}
