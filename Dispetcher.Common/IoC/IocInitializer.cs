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

                InitCustom(builder);

                container = builder.Build();
            }
        }

        private static void InitCustom(ContainerBuilder builder)
        {
            const string username = "csv@gde-edet.com";
            const string password = "Id4nInilrH2Ha";
        
            var mailClient = new YandexMailClient(username, password, "");
            builder.RegisterInstance(mailClient).As<IMailClient>();

            var checkMailboxTask = new CheckMailboxTask();
            builder.RegisterInstance(checkMailboxTask).AsSelf().As<ITask>();
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
                    var interfaceType = type.GetInterfaces().First(i => i.IsDefined(typeof(ComponentInterfaceAttribute)));
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
    }
}
