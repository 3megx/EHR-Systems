using EHRPlatform.Common.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EHRPlatform.Common.Extensions;

/// <summary>
/// OpenTelemetry tracing DI extensions for EHR microservices.
///
/// Instruments:
///   ASP.NET Core incoming requests
///   HttpClient outbound calls
///   EHR custom activities (EHRTelemetry.ActivitySource)
///
/// Exporters: OTLP (Jaeger/Grafana Tempo) or Console in development.
/// </summary>
public static class TelemetryExtensions
{
    /// <summary>
    /// Add OpenTelemetry tracing for an EHR microservice.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">App configuration.</param>
    /// <param name="serviceName">Logical service name (e.g. "patient-service").</param>
    public static IServiceCollection AddEHRTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        var otlpEndpoint = configuration["OpenTelemetry:OtlpEndpoint"];

        services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(
                    serviceName: serviceName,
                    serviceVersion: EHRTelemetry.ServiceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = configuration["ASPNETCORE_ENVIRONMENT"] ?? "production"
                }))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(EHRTelemetry.ServiceName)   // Custom EHR spans
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.RecordException = true;
                        // Exclude health check noise
                        opts.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health");
                    })
                    .AddHttpClientInstrumentation(opts =>
                    {
                        opts.RecordException = true;
                    });

                // Console exporter for development; swap for OTLP (Jaeger / Grafana Tempo)
                // in production by adding OpenTelemetry.Exporter.Otlp and calling:
                //   tracing.AddOtlpExporter(opts => opts.Endpoint = new Uri(otlpEndpoint));
                tracing.AddConsoleExporter();
            });

        return services;
    }
}
