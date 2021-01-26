using _2C2P.DEMO.WebApp.Middlewares.Jaeger;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;
using System.Reflection;

namespace _2C2P.DEMO.WebApp.Services.Jaeger
{
    public static class ServiceCollectionExtensions
    {
        private static bool _initialized;

        public static IServiceCollection AddJaeger(this IServiceCollection services)
        {
            if (_initialized)
            {
                return services;
            }

            _initialized = true;

            var options = GetJaegerOptions(services);
            if (!options.Enabled)
            {
                var defaultTracer = new Tracer.Builder(Assembly.GetEntryAssembly().FullName)
                    .WithReporter(new NoopReporter())
                    .WithSampler(new ConstSampler(false))
                    .Build();

                services.AddSingleton<ITracer>(defaultTracer);

                return services;
            }

            services.AddSingleton<ITracer>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                var reporter = new RemoteReporter.Builder()
                    .WithSender(new UdpSender(options.UdpHost, options.UdpPort, options.MaxPacketSize))
                    .WithLoggerFactory(loggerFactory)
                    .Build();

                var sampler = GetSampler(options);

                var tracer = new Tracer.Builder(options.ServiceName)
                    .WithReporter(reporter)
                    .WithSampler(sampler)
                    .Build();

                _ = GlobalTracer.RegisterIfAbsent(tracer);

                return tracer;
            });

            services.AddOpenTracing();

            return services;
        }

        private static ISampler GetSampler(JaegerOptions options)
        {
            return options.Sampler switch
            {
                "const" => new ConstSampler(true),
                "rate" => new RateLimitingSampler(options.MaxTracesPerSecond),
                "probabilistic" => new ProbabilisticSampler(options.SamplingRate),
                _ => new ConstSampler(true),
            };
        }

        private static JaegerOptions GetJaegerOptions(IServiceCollection services)
        {
            using var seriveProvider = services.BuildServiceProvider();

            var configuration = seriveProvider.GetService<IConfiguration>();
            var jaegerConfig = new JaegerOptions();

            configuration.Bind("Jaeger", jaegerConfig);

            return jaegerConfig;
        }
    }
}
