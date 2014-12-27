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
            var interfaceAttr = typeof(T).GetCustomAttribute<ComponentInterfaceAttribute>();
            if (interfaceAttr != null && interfaceAttr.AllowMultiple)
                throw new Exception(String.Format("Multiple components are allowed for type {0}", typeof(T)));

            var components = IocInitializer.CurrentContainer.Resolve<IEnumerable<T>>().ToList();
            if (interfaceAttr != null && !interfaceAttr.AllowMultiple && components.Count > 1)
                throw new Exception(String.Format("Multiple components are not allowed for type {0}", typeof(T)));

            return components.First();
        }

        /// <summary>
        /// Получить все реализации в соответствии с порядком
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            var interfaceAttr = typeof(T).GetCustomAttribute<ComponentInterfaceAttribute>();
            if (!interfaceAttr.AllowMultiple)
                throw new Exception(String.Format("Multiple components are not allowed for type {0}", typeof(T)));

            return IocInitializer.CurrentContainer.Resolve<IEnumerable<T>>().OrderBy(x =>
                {
                    var attr = x.GetType().GetCustomAttribute<ComponentAttribute>();
                    return attr.Order;
                });
        }
    }
}
