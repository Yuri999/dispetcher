using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispetcher.Common.Models
{
    /// <summary>
    /// Строка CSV файла
    /// </summary>
    public class CsvItem
    {
        public DateTime Date { get; set; }

        public string Schedule { get; set; }

        public string SideNumber { get; set; }

        public VehicleType VehicleType { get; set; }

        public string RouteName { get; set; }
    }
}
