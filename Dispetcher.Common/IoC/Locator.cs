using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.Reflection;

namespace Dispetcher.Common.IoC
{
    public static class Locator
    {
        public static T Resolve<T>() where T: class
        {
            return IocInitializer.CurrentContainer.Resolve<IEnumerable<T>>().OrderBy(x =>
                {
                    var attr = x.GetType().GetCustomAttribute<ComponentAttribute>();
                    return attr.Order;
                }).First();
        }
    }
}
