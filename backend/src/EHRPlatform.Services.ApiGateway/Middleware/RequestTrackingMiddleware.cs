using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace EHRPlatform.Services.ApiGateway.Middleware;

/// <summary>
/// Middleware for tracking requests through API Gateway.
/// Adds correlation ID, measures latency, logs request/response.
/// </summary>
public class RequestTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTrackingMiddleware> _logger;

    public RequestTrackingMiddleware(RequestDelegate next, ILogger<RequestTrackingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Generate correlation ID
        var correlationId = context.Request.Headers.ContainsKey("X-Correlation-ID")
            ? context.Request.Headers["X-Correlation-ID"].ToString()
            : Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Add("X-Correlation-ID", correlationId);

        // Add to Serilog context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        using (LogContext.PushProperty("RequestMethod", context.Request.Method))
        {
            var watch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                watch.Stop();

                _logger.LogInformation(
                    "Request {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    watch.ElapsedMilliseconds);
            }
        }
    }
}

/// <summary>
/// Extension methods for request tracking middleware.
/// </summary>
public static class RequestTrackingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTracking(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestTrackingMiddleware>();
    }
}
