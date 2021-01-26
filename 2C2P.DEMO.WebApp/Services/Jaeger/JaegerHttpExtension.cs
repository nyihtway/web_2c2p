using _2C2P.DEMO.WebApp.Middlewares.Jaeger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace _2C2P.DEMO.WebApp.Services.Jaeger
{
    public static class JaegerHttpExtension
    {
        public static IApplicationBuilder UseJaeger(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var jaegerConfig = new JaegerOptions();

            scope.ServiceProvider.GetService<IConfiguration>()?.Bind("Jaeger", jaegerConfig);

            return jaegerConfig.Enabled ? app.UseMiddleware<JaegerHttpMiddleware>() : app;
        }
    }
}
