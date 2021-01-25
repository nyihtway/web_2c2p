using _2C2P.DEMO.Infrastructure;
using _2C2P.DEMO.WebApp.Services;
using Autofac;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Linq;
using System.Reflection;

namespace _2C2P.DEMO.WebApp.AutofacModules
{
    public class InfrastructureModule : Autofac.Module
    {
        IMongoClient _mongoClient;
        string _dbName;

        public InfrastructureModule(IMongoClient client, string dbName)
        {
            _mongoClient = client;
            _dbName = dbName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("2C2P.DEMO.Infrastructure"))
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Register(ctx =>
            {
                return new MongoContext(_mongoClient, _dbName);
            }).As<IMongoContext>().SingleInstance();

            ConventionRegistry.Register(
                "Ignore null values",
                new ConventionPack
                {
                    new IgnoreIfNullConvention(true)
                },
                t => true);

            builder.RegisterType<CrudService>().As<ICrudService>().InstancePerLifetimeScope();
        }
    }
}
