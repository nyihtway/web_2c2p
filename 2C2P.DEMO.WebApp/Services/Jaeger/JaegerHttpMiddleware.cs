using Microsoft.AspNetCore.Http;
using OpenTracing;
using OpenTracing.Tag;
using System;
using System.Threading.Tasks;

namespace _2C2P.DEMO.WebApp.Services.Jaeger
{
    internal sealed class JaegerHttpMiddleware
    {
        private readonly RequestDelegate _next;

        public JaegerHttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITracer tracer)
        {
            IScope scope = null;
            var span = tracer.ActiveSpan;
            var method = context.Request.Method;

            if (span is null)
            {
                var spanBuilder = tracer.BuildSpan($"HTTP {method}::{context.Request.Path}");
                scope = spanBuilder.StartActive(true);
                span = scope.Span;
            }
            else
            {
                span.SetOperationName($"HTTP {method}::{context.Request.Path}");
            }

            span.Log($"Processing HTTP {method}: {context.Request.Path}");

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                span.SetTag(Tags.Error, true);
                span.Log(ex.Message);
                throw ex;
            }
            finally
            {
                scope?.Dispose();
            }
        }
    }
}
