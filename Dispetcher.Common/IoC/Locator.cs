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
            if (IocInitializer.CurrentContainer == null)
                throw new Exception("IoC не инициализирован.");

            var interfaceAttrs = typeof(T).GetCustomAttributes(typeof(ComponentInterfaceAttribute), false);
            if (interfaceAttrs != null && interfaceAttrs.Length > 0 && ((ComponentInterfaceAttribute)interfaceAttrs[0]).AllowMultiple)
                throw new Exception(String.Format("Multiple components are allowed for type {0}", typeof(T)));

            var components = IocInitializer.CurrentContainer.Resolve<IEnumerable<T>>().ToList();
            if (interfaceAttrs != null && interfaceAttrs.Length > 0 && !((ComponentInterfaceAttribute)interfaceAttrs[0]).AllowMultiple && components.Count > 1)
                throw new Exception(String.Format("Multiple components are not allowed for type {0}", typeof(T)));

            if (components.Count == 0)
                throw new Exception(String.Format("Невозможно разрешить зависимость для типа {0}.", typeof(T).ToString()));

            return components.First();
        }

        /// <summary>
        /// Получить все реализации в соответствии с порядком
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            if (IocInitializer.CurrentContainer == null)
                throw new Exception("IoC не инициализирован.");

            var interfaceAttrs = typeof(T).GetCustomAttributes(typeof(ComponentInterfaceAttribute), false);
            if (interfaceAttrs != null && interfaceAttrs.Length > 0 && !((ComponentInterfaceAttribute)interfaceAttrs[0]).AllowMultiple)
                throw new Exception(String.Format("Multiple components are not allowed for type {0}", typeof(T)));

            return IocInitializer.CurrentContainer.Resolve<IEnumerable<T>>().OrderBy(x =>
            {
                var attrs = x.GetType().GetCustomAttributes(typeof (ComponentAttribute), false);
                return ((ComponentAttribute) attrs[0]).Order;
            });
        }
    }
}
