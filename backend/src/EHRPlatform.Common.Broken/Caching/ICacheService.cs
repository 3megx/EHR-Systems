using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EHRPlatform.Common.Caching;

/// <summary>
/// Cache interface for distributed caching with Redis.
/// Provides get/set/remove operations with pattern-based invalidation.
/// Supports TTL-based expiration for automatic cache cleanup.
/// 
/// HIPAA Note: Cache should not store sensitive PII without encryption.
/// Use data masking before caching patient-specific queries.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get value from cache by key.
    /// Returns null if key not found or expired.
    /// </summary>
    /// <typeparam name="T">Type of cached value (must be JSON-serializable)</typeparam>
    /// <param name="key">Cache key (e.g., "patient:123")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached value or null if not found</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Set value in cache with optional expiration.
    /// If key exists, overwrites the value.
    /// </summary>
    /// <typeparam name="T">Type of value to cache</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <param name="expiration">Optional TTL. If null, uses default duration (5 minutes).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Remove single key from cache.
    /// Does not error if key doesn't exist.
    /// </summary>
    /// <param name="key">Cache key to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove multiple keys in batch.
    /// Does not error if keys don't exist.
    /// </summary>
    /// <param name="keys">Collection of cache keys to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove all keys matching a pattern using Redis SCAN.
    /// Useful for bulk invalidation (e.g., "patient:*" to clear all patient caches).
    /// 
    /// Warning: Pattern matching can be expensive on large caches.
    /// Use specific patterns when possible.
    /// 
    /// Common patterns:
    /// - "patient:*" - all patient caches
    /// - "appointment:*" - all appointment caches
    /// - "patient:123:*" - all caches for patient 123
    /// </summary>
    /// <param name="pattern">Redis glob pattern (*, ?, [])</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of keys removed</returns>
    Task<long> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if key exists in cache.
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get or set value atomically.
    /// If key exists, returns cached value.
    /// If not, executes factory function, caches result, and returns.
    /// Useful for avoiding thundering herd problem.
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="factory">Factory function to create value if not cached</param>
    /// <param name="expiration">Cache duration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Get multiple values by keys efficiently using pipeline.
    /// Returns dictionary with keys present in cache.
    /// Missing keys are not included in result.
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    /// <param name="keys">Collection of cache keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of key-value pairs</returns>
    Task<Dictionary<string, T>> GetManyAsync<T>(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Set expiration time on existing key.
    /// Useful for extending cache duration or implementing LRU-like behavior.
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="expiration">New TTL duration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if key exists and expiration was set, false if key not found</returns>
    Task<bool> ExpireAsync(
        string key,
        TimeSpan expiration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get time to live (TTL) for key.
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>TimeSpan remaining, or null if key doesn't exist or has no expiration</returns>
    Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear entire cache.
    /// WARNING: This affects all applications using Redis.
    /// Use only during maintenance or testing.
    /// </summary>
    Task FlushAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get cache statistics for monitoring.
    /// Includes hit rate, memory usage, key count, etc.
    /// </summary>
    Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Marker interface for queries that should be cached.
/// Automatically cached by CachingBehavior.
/// 
/// Usage:
/// public class GetPatientQuery : IQuery<PatientDto>, ICachedQuery
/// {
///     public string CacheKey => $"patient:{PatientId}";
///     public TimeSpan? Duration => TimeSpan.FromMinutes(5);
/// }
/// </summary>
public interface ICachedQuery
{
    /// <summary>
    /// Unique cache key for this query result.
    /// Should include all parameters that affect the result.
    /// 
    /// Examples:
    /// - $"patient:{Id}" - single patient
    /// - $"patients:page:{Page}:size:{Size}" - paginated list
    /// - $"patients:search:{SearchTerm}:page:{Page}" - with search
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Cache duration (TTL).
    /// If null, uses ICacheService default (5 minutes).
    /// Set to TimeSpan.Zero for no caching.
    /// </summary>
    TimeSpan? Duration { get; }
}

/// <summary>
/// Cache statistics for monitoring and optimization.
/// Retrieved via GetStatisticsAsync().
/// </summary>
public class CacheStatistics
{
    public long HitCount { get; set; }
    public long MissCount { get; set; }
    public long KeyCount { get; set; }
    public long MemoryUsedBytes { get; set; }
    public double HitRate => HitCount + MissCount > 0 
        ? (double)HitCount / (HitCount + MissCount) 
        : 0;
    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
}
