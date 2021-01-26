using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Net;

namespace _2C2P.DEMO.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", true, true)
                 .Build();

            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(config)
              .CreateLogger();

            var logger = Log.ForContext<Program>();

            try
            {
                logger.Information("Start webhost");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Fatal("Webhost terminated unexpectedly {@ex}", ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, config) =>
                    {
                        config.Listen(IPAddress.Any, 5001);
                    });
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog();
    }
}
