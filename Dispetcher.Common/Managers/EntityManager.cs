using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispetcher.Common.IoC;
using Dispetcher.Common.Models;

namespace Dispetcher.Common.Managers
{
    [ComponentInterface(AllowMultiple = true, LifeTime = ComponentLifeTime.Singleton)]
    public interface IEntityManager<T> where T: class
    {
        void Insert(T obj);

        IEnumerable<T> Select(string whereClause, Dictionary<string, object> parameters = null);

        int Delete(string whereClause, Dictionary<string, object> parameters = null);

        void Delete(CsvItem obj);

        void Update(long id, Dictionary<string, object> fieldValues);
    }
}
