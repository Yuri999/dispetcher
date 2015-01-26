using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispetcher.Common.Database;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Models;
using Microsoft.Win32.SafeHandles;

namespace Dispetcher.Common.Managers
{
    [Component]
    public class CsvItemManager : IEntityManager<CsvItem>
    {
        private Lazy<IDbManager> dbManagerLazy = new Lazy<IDbManager>(() => Locator.Resolve<IDbManager>());
        private IDbManager DbManager { get { return dbManagerLazy.Value; } }

        public void Insert(CsvItem obj)
        {
            var id = DbManager.ExecQuery<long>("INSERT INTO [Journal] (Date, SideNumberPlan, SideNumberFact, Schedule, Route, VehicleType, Protected) " +
                        "VALUES (@date, @sidenumberplan, @sidenumberfact, @schedule, @route, @vehicletype, @protected); SELECT last_insert_rowid();",
                new Dictionary<string, object>()
                            {
                                { "date", obj.Date }, 
                                { "sidenumberplan", obj.SideNumberPlan },
                                { "sidenumberfact", obj.SideNumberFact },
                                { "schedule", obj.Schedule },
                                { "route", obj.Route },
                                { "vehicletype", obj.VehicleType },
                                { "protected", false },
                            }).First();
            obj.Id = id;
        }

        public IEnumerable<CsvItem> Select(string whereClause, Dictionary<string, object> parameters = null)
        {
            return DbManager.ExecQuery<CsvItem>(String.Format("SELECT * FROM [Journal] WHERE {0}", whereClause), parameters);
        }

        public int Delete(string whereClause, Dictionary<string, object> parameters = null)
        {
            return DbManager.ExecNonQuery(String.Format("DELETE FROM [Journal] WHERE {0}", whereClause), parameters);
        }

        public void Delete(CsvItem obj)
        {
            DbManager.ExecNonQuery("DELETE FROM [Journal] WHERE Id = @id", new Dictionary<string, object>() {{"id", obj.Id}});
        }

        public void Update(long id, Dictionary<string, object> fieldValues)
        {
            var parameters = new Dictionary<string, object>() {{"id", id}};
            var sb = new StringBuilder();
            foreach (var fieldValue in fieldValues)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(String.Format("{0}=@{0}", fieldValue.Key));
                parameters.Add(fieldValue.Key, fieldValue.Value);
            }

            DbManager.ExecNonQuery(String.Format("UPDATE [Journal] SET {0} WHERE Id=@id", sb.ToString()), parameters);
        }
    }
}
