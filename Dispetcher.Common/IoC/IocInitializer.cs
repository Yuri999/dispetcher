using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Dispetcher.Common.Mail;
using Dispetcher.Common.Tasks;

namespace Dispetcher.Common.IoC
{
    using System.IO;

    public class IocInitializer
    {
        private static readonly object syncLock = new object();
        private static IContainer container;

        public static IContainer CurrentContainer
        {
            get
            {
                return container;
            }
        }

        public static void Init()
        {
            lock (syncLock)
            {
                LoadAssemblies();

                var builder = new Autofac.ContainerBuilder();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.IsDefined(typeof(ComponentAssemblyAttribute), false))
                        ProcessAssembly(builder, assembly);
                }

                container = builder.Build();
            }
        }

        private static void LoadAssemblies()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.Location).ToList();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (string dll in Directory.GetFiles(path, "*.dll"))
            {
                if (loadedAssemblies.Contains(dll))
                    continue;

                try
                {
                    Assembly.LoadFile(dll);
                    loadedAssemblies.Add(dll);
                }
                catch (Exception)
                {
                }
            }            
        }

        private static void ProcessAssembly(ContainerBuilder builder, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsDefined(typeof(ComponentAttribute)))
                {
                    var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.IsDefined(typeof(ComponentInterfaceAttribute)));
                    if (interfaceType == null)
                    {
                        throw new Exception(String.Format("Не удалось найти родительский интерфейс с атрибутом [ComponentInterface] для типа {0}.", type.ToString()));
                    }

                    var reg = builder.RegisterType(type).AsSelf().As(interfaceType);
                    var interfaceAttribute = interfaceType.GetCustomAttribute<ComponentInterfaceAttribute>();
                    switch (interfaceAttribute.LifeTime)
                    {
                        case ComponentLifeTime.Singleton:
                            reg.SingleInstance();
                            break;
                        case ComponentLifeTime.Transient:
                            reg.InstancePerDependency();
                            break;
                    }
                }
            }
        }

        public static void RegisterInstance<T>(T instance) where T:class
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(instance).AsSelf();
            builder.Update(container);
        }
    }
}
