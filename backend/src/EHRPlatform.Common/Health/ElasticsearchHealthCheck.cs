using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

// Disambiguate HealthStatus — Elastic.Clients.Elasticsearch also defines one.
using HealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus;

namespace EHRPlatform.Common.Health;

/// <summary>
/// Health check for Elasticsearch connectivity.
/// Uses the cluster ping endpoint.
/// </summary>
public sealed class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchHealthCheck> _logger;

    public ElasticsearchHealthCheck(
        ElasticsearchClient client,
        ILogger<ElasticsearchHealthCheck> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PingAsync(cancellationToken: cancellationToken);

            if (response.IsSuccess())
                return HealthCheckResult.Healthy("Elasticsearch is reachable");

            var error = response.ApiCallDetails?.OriginalException?.Message ?? "Unknown error";
            _logger.LogWarning("Elasticsearch health check degraded: {Error}", error);
            return HealthCheckResult.Degraded($"Elasticsearch ping failed: {error}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Elasticsearch health check threw an exception");
            return HealthCheckResult.Unhealthy("Elasticsearch is unreachable", ex);
        }
    }
}

/// <summary>Extension helpers for registering Elasticsearch health checks.</summary>
public static class ElasticsearchHealthCheckExtensions
{
    public static IHealthChecksBuilder AddElasticsearchHealthCheck(
        this IHealthChecksBuilder builder,
        string? name = null,
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
    {
        return builder.AddCheck<ElasticsearchHealthCheck>(
            name          ?? "elasticsearch",
            failureStatus ?? HealthStatus.Degraded,
            tags          ?? new[] { "search", "elasticsearch" },
            timeout       ?? TimeSpan.FromSeconds(5));
    }
}
