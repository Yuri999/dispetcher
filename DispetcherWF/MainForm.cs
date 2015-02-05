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
using Dispetcher.Common.Tasks;

namespace DispetcherWF
{
    public partial class MainForm : Form
    {
        #region Services
        private Lazy<CsvFileProcessor> _csvFileProcessorLazy = new Lazy<CsvFileProcessor>(() => Locator.Resolve<CsvFileProcessor>());
        private CsvFileProcessor CsvFileProcessor { get { return _csvFileProcessorLazy.Value; } }

        private Lazy<CheckMailboxTask> _checkMailboxTaskLazy = new Lazy<CheckMailboxTask>(() => Locator.Resolve<CheckMailboxTask>());
        private CheckMailboxTask CheckMailboxTask { get { return _checkMailboxTaskLazy.Value; } }

        private Lazy<IDbManager> dbManagerLazy = new Lazy<IDbManager>(() => Locator.Resolve<IDbManager>());
        private IDbManager DbManager { get { return dbManagerLazy.Value; } }
        #endregion

        private const string UPDATE_TEXT_TEMPLATE = "Последнее обновление данных: {0} в {1}.";
        private const string CHECKMAIL_TEXT_TEMPLATE = "Проверка почты: {0}.";
        private const string CHECKMAIL_TEXT_SUCCESS = "работает";
        private const string CHECKMAIL_TEXT_ERROR = "завершилась с ошибками";

        public MainForm()
        {
            InitializeComponent();

            try
            {
                LoadData();
                CheckData();
                CsvFileProcessor.BeforeDataChange += CsvFileProcessorOnBeforeDataChange;
                CsvFileProcessor.AfterDataChange += CsvFileProcessorOnAfterDataChange;
                CheckMailboxTask.Check += CheckMailboxTaskOnCheck;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка запуска");
            }
        }

        private void LoadData()
        {
            // до четырех утра считаем что это еще предыдущий день
            var date = DateTime.Now.Hour < 4 ? DateTime.Today.AddDays(-1) : DateTime.Today;

            dataSet1.Tables[0].Rows.Clear();
            
            var items = DbManager.ExecQuery<CsvItem>("SELECT * FROM [Journal] WHERE Date = @date ORDER BY VehicleType, CAST(Route as integer)",
                new Dictionary<string, object>() { { "date", date } }).ToList();

            foreach (var csvItem in items)
            {
                var row = dataSet1.Tables[0].NewRow();
                row["Id"] = csvItem.Id;
                row["Route"] = String.Format("{0} {1}", csvItem.VehicleType == VehicleType.Трамвай ? "ТР" : "ТБ", csvItem.Route);
                row["SideNumberPlan"] = csvItem.SideNumberPlan;
                row["SideNumberFact"] = csvItem.SideNumberFact;
                row["Schedule"] = csvItem.Schedule;
                row["VehicleType"] = csvItem.VehicleType;
                dataSet1.Tables[0].Rows.Add(row);
            }
        }

        private void CheckData()
        {
            var d = new Dictionary<string, List<DataRow>>();

            foreach (DataRow row in dataSet1.Tables[0].Rows)
            {
                var key = String.Format("{0}|{1}", row["VehicleType"], row["SideNumberFact"]);

                if (!d.ContainsKey(key))
                {
                    d[key] = new List<DataRow>() { row };
                }
                else
                {
                    d[key].Add(row);
                }
            }
            var sb = new StringBuilder();
            var errorNumbers = d.Where(x => !string.IsNullOrWhiteSpace(x.Key) && x.Value.Count > 1);
            foreach (var errPair in errorNumbers)
            {
                var vt = Convert.ToString(errPair.Value.First()["VehicleType"]);
                var num = Convert.ToString(errPair.Value.First()["SideNumberFact"]);
                if (String.IsNullOrWhiteSpace(num))
                    continue;

                sb.AppendLine(String.Format("{0} № {1} заявлен на несколько расписаний:", vt, num));
                foreach (var item in errPair.Value)
                {
                    sb.AppendLine(String.Format(" - маршрут {0} (расписание {1})", item["Route"], item["Schedule"]));
                }
            }

            if (sb.Length > 0)
            {
                tbErrors.Text = sb.ToString();
                pnErrors.Visible = true;
            }
            else
            {
                tbErrors.Text = "";
                pnErrors.Visible = false;
            }
        }

        private void CsvFileProcessorOnAfterDataChange()
        {
            this.Invoke(new Action(LoadData));
            this.Invoke(new Action(CheckData));
            this.Invoke(new Action(() => { lbUpdateStatus.Text = String.Format(UPDATE_TEXT_TEMPLATE, DateTime.Today.ToString("dd.MM.yyyy"), DateTime.Now.ToString("HH:mm:ss")); }));
            Locator.Resolve<SendChangesTask>().QueueSend();
        }

        private void CheckMailboxTaskOnCheck(object sender, CheckMailEventArgs checkMailEventArgs)
        {
            this.Invoke(new Action(() => { lbCheckMailStatus.Text = String.Format(CHECKMAIL_TEXT_TEMPLATE, 
                checkMailEventArgs.CheckResult.Exceptions.Any() ? CHECKMAIL_TEXT_ERROR : CHECKMAIL_TEXT_SUCCESS); }));
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
                if (newValue == sideNumberFact_BeginEdit_value)
                    return;
                
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

                CheckData();
                Locator.Resolve<SendChangesTask>().QueueSend();
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

        private void UpdateStatistics()
        {
            var d = new Dictionary<string, int>();
            foreach (DataRow row in dataSet1.Tables[0].Rows)
            {
                var route = Convert.ToString(row["Route"]);
                if (!d.ContainsKey(route))
                {
                    d[route] = 0;
                }

                if (!String.IsNullOrWhiteSpace(Convert.ToString(row["SideNumberFact"])))
                {
                    d[route]++;
                }
            }

            var sb = new StringBuilder();
            foreach (var keyValue in d)
            {
                sb.AppendLine(String.Format("{0} - {1} шт", keyValue.Key, keyValue.Value));
            }

            tbStatistics.Text = sb.ToString();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.TabIndex == tabPage2.TabIndex)
            {
                UpdateStatistics();
            }
        }
    }
}
