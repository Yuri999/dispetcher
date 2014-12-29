using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dispetcher.Common.Database;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Models;
using Dispetcher.Common.Processor;

namespace DispetcherWF
{
    public partial class MainForm : Form
    {
        #region Services
        private Lazy<CsvFileProcessor> _csvFileProcessorLazy = new Lazy<CsvFileProcessor>(() => Locator.Resolve<CsvFileProcessor>());
        private CsvFileProcessor CsvFileProcessor { get { return _csvFileProcessorLazy.Value; } }

        private Lazy<IDbManager> dbManagerLazy = new Lazy<IDbManager>(() => Locator.Resolve<IDbManager>());
        private IDbManager DbManager { get { return dbManagerLazy.Value; } }
        #endregion

        public MainForm()
        {
            InitializeComponent();

            try
            {
                LoadData();
                CsvFileProcessor.BeforeDataChange += CsvFileProcessorOnBeforeDataChange;
                CsvFileProcessor.AfterDataChange += CsvFileProcessorOnAfterDataChange;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка запуска");
            }
        }

        private void LoadData()
        {
            dataSet1.Tables[0].Clear();
            
            var items = DbManager.ExecQuery<CsvItem>("SELECT * FROM [Journal] WHERE Date = @date ORDER BY VehicleType, Route",
                new Dictionary<string, object>() { { "date", /*DateTime.Today*/ new DateTime(2014, 09, 07) } }).ToList();

            foreach (var csvItem in items)
            {
                var row = dataSet1.Tables[0].NewRow();
                row["Route"] = String.Format("{0} {1}", csvItem.VehicleType == VehicleType.Трамвай ? "ТР" : "ТБ", csvItem.Route);
                row["SideNumberPlan"] = csvItem.SideNumberPlan;
                row["SideNumberFact"] = csvItem.SideNumberFact;
                row["Schedule"] = csvItem.Schedule;
                dataSet1.Tables[0].Rows.Add(row);
            }
        }

        private void CsvFileProcessorOnAfterDataChange()
        {
            LoadData();
        }

        private void CsvFileProcessorOnBeforeDataChange()
        {

        }
    }
}
