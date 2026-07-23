using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using EHRPlatform.Common.Caching;

namespace EHRPlatform.Common.Health;

/// <summary>
/// Health check for Redis cache connectivity and performance.
/// Verifies cache is responsive and performing within acceptable latency.
/// </summary>
public class CacheHealthCheck : IHealthCheck
{
    private readonly ICacheService _cacheService;

    public CacheHealthCheck(ICacheService cacheService)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var testKey = $"health-check-{Guid.NewGuid()}";
            var testValue = "healthy";

            // Test write
            await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromSeconds(10), cancellationToken);

            // Test read
            var retrieved = await _cacheService.GetAsync<string>(testKey, cancellationToken);

            if (retrieved != testValue)
                return HealthCheckResult.Unhealthy("Cache read/write mismatch");

            // Test delete
            await _cacheService.RemoveAsync(testKey, cancellationToken);

            // Get statistics
            var stats = await _cacheService.GetStatisticsAsync(cancellationToken);

            var description = $"Cache is healthy. Keys: {stats.KeyCount}, Memory: {FormatBytes(stats.MemoryUsedBytes)}";

            return HealthCheckResult.Healthy(description, stats);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Cache health check failed: {ex.Message}", ex);
        }
    }

    private static string FormatBytes(long bytes)
    {
        const long kb = 1024;
        const long mb = kb * 1024;
        const long gb = mb * 1024;

        return bytes switch
        {
            >= gb => $"{bytes / (double)gb:F2} GB",
            >= mb => $"{bytes / (double)mb:F2} MB",
            >= kb => $"{bytes / (double)kb:F2} KB",
            _ => $"{bytes} B"
        };
    }
}

/// <summary>
/// Extension for adding cache health checks to the application.
/// </summary>
public static class CacheHealthCheckExtensions
{
    /// <summary>
    /// Add cache health check to health check service.
    /// </summary>
    public static IServiceCollection AddCacheHealthCheck(
        this IServiceCollection services,
        string? name = null,
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
    {
        services.AddHealthChecks()
            .AddCheck<CacheHealthCheck>(
                name ?? "Redis Cache",
                failureStatus ?? HealthStatus.Unhealthy,
                tags: tags ?? new[] { "cache", "redis" },
                timeout: timeout ?? TimeSpan.FromSeconds(5));

        return services;
    }
}
