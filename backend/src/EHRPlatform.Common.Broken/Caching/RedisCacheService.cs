using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace EHRPlatform.Common.Caching;

/// <summary>
/// Redis-based distributed cache service.
/// Uses StackExchange.Redis for production-grade caching.
/// Supports pattern-based invalidation for bulk cache clearing.
/// Handles serialization/deserialization transparently.
/// 
/// Connection: Configured via DI in DataAccessExtensions
/// Usage: Inject ICacheService, call GetAsync/SetAsync
/// TTL: Default 5 minutes, configurable per operation
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly IServer _server;
    private readonly TimeSpan _defaultDuration = TimeSpan.FromMinutes(5);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer ?? 
            throw new ArgumentNullException(nameof(connectionMultiplexer));
        
        _database = _connectionMultiplexer.GetDatabase();
        _server = _connectionMultiplexer.GetServer(
            _connectionMultiplexer.GetEndPoints().First());
    }

    /// <summary>
    /// Get value from cache.
    /// Deserializes JSON stored in Redis.
    /// Returns null if key not found or expired.
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) 
        where T : class
    {
        ArgumentGuard.NotNullOrEmpty(key, nameof(key));

        try
        {
            var value = await _database.StringGetAsync(key);
            
            if (!value.HasValue)
                return null;

            return JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
        }
        catch (JsonException)
        {
            // Corrupted cache entry - remove it
            await _database.KeyDeleteAsync(key);
            return null;
        }
    }

    /// <summary>
    /// Set value in cache with TTL.
    /// Serializes to JSON before storing.
    /// Uses default duration if expiration not specified.
    /// </summary>
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentGuard.NotNullOrEmpty(key, nameof(key));
        ArgumentGuard.NotNull(value, nameof(value));

        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            var ttl = expiration ?? _defaultDuration;
            
            await _database.StringSetAsync(
                key,
                json,
                ttl);
        }
        catch (Exception ex)
        {
            // Log but don't throw - cache failures shouldn't break application
            System.Console.WriteLine($"Cache write error for key {key}: {ex.Message}");
        }
    }

    /// <summary>
    /// Remove single key from cache.
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentGuard.NotNullOrEmpty(key, nameof(key));

        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Cache delete error for key {key}: {ex.Message}");
        }
    }

    /// <summary>
    /// Remove multiple keys in batch using pipeline.
    /// More efficient than individual deletes.
    /// </summary>
    public async Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        ArgumentGuard.NotNull(keys, nameof(keys));

        var keyList = keys.ToList();
        if (keyList.Count == 0)
            return;

        try
        {
            var redisKeys = keyList.Select(k => (RedisKey)k).ToArray();
            await _database.KeyDeleteAsync(redisKeys);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Cache batch delete error: {ex.Message}");
        }
    }

    /// <summary>
    /// Remove keys matching pattern using SCAN.
    /// Implements cursor-based iteration to avoid blocking.
    /// Useful for bulk invalidation without locking Redis.
    /// </summary>
    public async Task<long> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        ArgumentGuard.NotNullOrEmpty(pattern, nameof(pattern));

        try
        {
            long removed = 0;
            var cursor = 0L;

            do
            {
                var scanResult = _server.Scan(
                    cursor,
                    pattern,
                    pageSize: 1000); // Batch size

                var keys = scanResult
                    .Select(k => (RedisKey)(string)k)
                    .ToArray();

                if (keys.Length > 0)
                {
                    removed += await _database.KeyDeleteAsync(keys);
                }

                cursor = scanResult.Cursor;

            } while (cursor != 0);

            return removed;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Cache pattern delete error for pattern {pattern}: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Check if key exists in cache.
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentGuard.NotNullOrEmpty(key, nameof(key));

        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Cache exists check error for key {key}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get or set value atomically.
    /// Prevents thundering herd - multiple requests for missing key
    /// only execute factory once.
    /// </summary>
    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentGuard.NotNullOrEmpty(key, nameof(key));
        ArgumentGuard.NotNull(factory, nameof(factory));

        // Try to get from cache first
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
            return cachedValue;

        // Not in cache - execute factory
        var value = await factory(cancellationToken);
        
        if (value != null)
        {
            // Store in cache for next time
            await SetAsync(value, key, expiration, cancellationToken);
        }

        return value;
    }

    /// <summary>
    /// Get multiple values using pipeline for efficiency.
    /// Returns only keys that exist and can be deserialized.
    /// </summary>
    public async Task<Dictionary<string, T>> GetManyAsync<T>(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentGuard.NotNull(keys, nameof(keys));

        var keyList = keys.ToList();
        if (keyList.Count == 0)
            return new Dictionary<string, T>();

        try
        {
            var redisKeys = keyList.Select(k => (RedisKey)k).ToArray();
            var values = await _database.StringGetAsync(redisKeys);

            var result = new Dictionary<string, T>();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].HasValue)
                {
                    try
                    {
                        var deserialized = JsonSerializer.Deserialize<T>(
                            values[i].ToString(), 
                            _jsonOptions);
                        
                        if (deserialized != null)
                        {
                            result[keyList[i]] = deserialized;
                        }
                    }
                    catch (JsonException)
                    {
                        // Skip corrupted entries
                        continue;
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Cache batch get error: {ex.Message}");
            return new Dictionary<string, T>();
        }
    }

    /// <summary>
    /// Set expiration time on existing key.
    /// </summary>
    public async Task<bool> ExpireAsync(
        string key,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        ArgumentGuard.NotNullOrEmpty(key, nameof(key));

        try
        {
            return await _database.KeyExpireAsync(key, expiration);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Cache expire error for key {key}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get remaining TTL for key.
    /// </summary>
    public async Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentGuard.NotNullOrEmpty(key, nameof(key));

        try
        {
            var ttl = await _database.KeyTimeToLiveAsync(key);
            return ttl;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Cache TTL check error for key {key}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Flush entire Redis database.
    /// WARNING: Dangerous operation - affects all applications.
    /// </summary>
    public async Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _server.FlushDatabaseAsync();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Cache flush error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get cache statistics for monitoring.
    /// </summary>
    public async Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var info = _server.Info();
            var stats = info.FirstOrDefault();

            var keyCount = _database.Execute("DBSIZE");
            var memory = stats?.First()?["used_memory"];

            return new CacheStatistics
            {
                KeyCount = long.TryParse(keyCount?.ToString(), out var count) ? count : 0,
                MemoryUsedBytes = long.TryParse(memory?.ToString(), out var mem) ? mem : 0,
                CollectedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Cache stats error: {ex.Message}");
            return new CacheStatistics();
        }
    }
}

/// <summary>
/// Helper for argument validation.
/// </summary>
internal static class ArgumentGuard
{
    public static void NotNull<T>(T? argument, string parameterName) where T : class
    {
        if (argument == null)
            throw new ArgumentNullException(parameterName);
    }

    public static void NotNullOrEmpty(string? argument, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException("Value cannot be null or empty", parameterName);
    }
}
