namespace EHRPlatform.Common.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using EHRPlatform.Common.Caching;

/// <summary>
/// MediatR pipeline behavior for automatic query result caching.
/// Only applies to queries implementing ICachedQuery interface.
/// 
/// Behavior:
/// 1. Check if request implements ICachedQuery
/// 2. If yes, check Redis cache
/// 3. If cache hit, return cached value
/// 4. If cache miss, execute handler and cache result
/// 5. Use configured TTL (default 5 minutes)
/// 
/// HIPAA Note:
/// Cache may contain PII if query returns patient data.
/// Use data masking before caching sensitive queries.
/// </summary>
/// <typeparam name="TRequest">The request type (must be IRequest&lt;TResponse&gt;)</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        ICacheService cacheService,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger;
    }

    /// <summary>
    /// Handle request with caching support.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only cache if request implements ICachedQuery
        if (request is not ICachedQuery cachedQuery)
        {
            return await next();
        }

        var cacheKey = cachedQuery.CacheKey;
        var duration = cachedQuery.Duration;

        try
        {
            // Try to get from cache
            var cachedValue = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
            if (cachedValue != null)
            {
                _logger.LogDebug("Cache HIT for key: {CacheKey}", cacheKey);
                return cachedValue;
            }

            _logger.LogDebug("Cache MISS for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            // Cache read error - continue with handler
            _logger.LogWarning(ex, "Cache read error for key {CacheKey}, executing handler", cacheKey);
        }

        // Not in cache - execute handler
        var response = await next();

        // Cache successful response
        if (response is not null)
        {
            try
            {
                await _cacheService.SetAsync(cacheKey, response, duration, cancellationToken);
                _logger.LogDebug("Cached response for key {CacheKey} with TTL {Seconds}s",
                    cacheKey,
                    duration?.TotalSeconds ?? 300);
            }
            catch (Exception ex)
            {
                // Cache write error - log but don't throw
                _logger.LogWarning(ex, "Cache write error for key {CacheKey}", cacheKey);
            }
        }

        return response;
    }
}
