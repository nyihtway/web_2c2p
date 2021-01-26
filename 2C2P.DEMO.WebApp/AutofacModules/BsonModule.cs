using _2C2P.DEMO.Infrastructure.BsonMapper;
using Autofac;
using System;
using System.Linq;

namespace _2C2P.DEMO.WebApp.AutofacModules
{
    public class BsonModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var runningAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.Contains("2C2P.DEMO."));
            foreach (var assembly in runningAssemblies)
            {
                var types = assembly.GetExportedTypes()
                    .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IBsonMapper<>)))
                    .ToList();

                foreach (var type in types)
                {
                    var tempType = type;
                    var instance = Activator.CreateInstance(tempType);
                    var methodInfo = tempType.GetMethod("Map") ?? tempType.GetInterface("IBsonMapper`1").GetMethod("Map");

                    methodInfo?.Invoke(instance, null);
                }
            }
        }
    }
}
