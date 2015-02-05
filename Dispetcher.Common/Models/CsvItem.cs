using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dispetcher.Common.Models
{
    /// <summary>
    /// Строка CSV файла
    /// </summary>
    public class CsvItem
    {
        public long Id { get; set; }

        public DateTime Date { get; set; }

        public string Schedule { get; set; }

        public string SideNumberPlan { get; set; }

        public string SideNumberFact { get; set; }

        public VehicleType VehicleType { get; set; }

        public string Route { get; set; }

        public bool Protected { get; set; }

        public DateTime ModifyDate { get; set; }

        public string CompositeRouteKey { get { return String.Format("{0}|{1}|{2}|{3}", Date.ToShortDateString(), Route, VehicleType, Schedule); } }

        public override string ToString()
        {
            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}", Date.ToShortDateString(), Route, VehicleType, Schedule, SideNumberPlan, SideNumberFact);
        }
    }
}
