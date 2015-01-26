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
            dataSet1.Tables[0].Rows.Clear();
            
            var items = DbManager.ExecQuery<CsvItem>("SELECT * FROM [Journal] WHERE Date = @date ORDER BY VehicleType, Route",
                new Dictionary<string, object>() { { "date", /*DateTime.Today*/ new DateTime(2014, 09, 07) } }).ToList();

            foreach (var csvItem in items)
            {
                var row = dataSet1.Tables[0].NewRow();
                row["Id"] = csvItem.Id;
                row["Route"] = String.Format("{0} {1}", csvItem.VehicleType == VehicleType.Трамвай ? "ТР" : "ТБ", csvItem.Route);
                row["SideNumberPlan"] = csvItem.SideNumberPlan;
                row["SideNumberFact"] = csvItem.SideNumberFact;
                row["Schedule"] = csvItem.Schedule;
                dataSet1.Tables[0].Rows.Add(row);
            }
        }

        private void CsvFileProcessorOnAfterDataChange()
        {
            this.Invoke(new Action(LoadData));
        }

        private void CsvFileProcessorOnBeforeDataChange()
        {
            MessageBox.Show("Получены новые данные.");
        }

        private string sideNumberFact_BeginEdit_value;

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "SideNumberFact")
            {
                sideNumberFact_BeginEdit_value = Convert.ToString(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "SideNumberFact")
            {
                var newValue = Convert.ToString(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
                if (newValue != sideNumberFact_BeginEdit_value)
                {
                    var transaction = DbManager.BeginTransaction();
                    try
                    {
                        var id = Convert.ToInt64(dataGridView1["Id", e.RowIndex].Value);

                        DbManager.ExecNonQuery("INSERT INTO [UserLog] (Date, ActionType, RecId, OldValue, NewValue) " +
                                               "VALUES (@date, @actionType, @recId, @oldValue, @newValue)",
                            new Dictionary<string, object>()
                            {
                                {"date", DateTime.Now},
                                {"actionType", UserActionType.SideNumberFactChanged},
                                {"recId", id},
                                {"oldValue", sideNumberFact_BeginEdit_value},
                                {"newValue", newValue}
                            });

                        DbManager.ExecNonQuery("UPDATE [Journal] SET SideNumberFact=@fact, Protected=1 WHERE Id=@id",
                            new Dictionary<string, object>()
                            {
                                {"fact", newValue},
                                {"id", id}
                            });

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // TODO log
                        transaction.Rollback();
                        throw;
                    }

                    dataGridView1.Refresh();
                }
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex > -1 && dataGridView1["SideNumberPlan", e.RowIndex].Value.ToString() != dataGridView1["SideNumberFact", e.RowIndex].Value.ToString())
            {
                var brush = dataGridView1[e.ColumnIndex, e.RowIndex].Selected ? new SolidBrush(Color.Pink) : new SolidBrush(Color.Orange);
                var bounds = new Rectangle(e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width-1, e.CellBounds.Height-1);
                e.Graphics.FillRectangle(brush, bounds);
                e.PaintContent(e.CellBounds);
                e.Handled = true;
            }
        }

    }
}
