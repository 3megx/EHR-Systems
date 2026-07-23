using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Serilog;
using EHRPlatform.Common.Caching;
using EHRPlatform.Common.Security;
using EHRPlatform.Common.Health;
using EHRPlatform.Common.Behaviors;
using MediatR;

namespace EHRPlatform.Common.Extensions;

/// <summary>
/// Configuration options for EHR Common services.
/// </summary>
public class EHRCommonOptions
{
    /// <summary>
    /// Redis connection string (e.g., "localhost:6379,password=secret").
    /// </summary>
    public string RedisConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Encryption key for sensitive data (must be 32+ characters).
    /// </summary>
    public string EncryptionKey { get; set; } = string.Empty;

    /// <summary>
    /// Enable or disable caching (default: true).
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Enable or disable encryption (default: true).
    /// </summary>
    public bool EnableEncryption { get; set; } = true;

    /// <summary>
    /// Enable or disable Serilog logging (default: true).
    /// </summary>
    public bool EnableLogging { get; set; } = true;
}

/// <summary>
/// Extension methods for registering EHR Common services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all EHR Common infrastructure services.
    /// </summary>
    public static IServiceCollection AddEHRCommon(
        this IServiceCollection services,
        IConfiguration configuration,
        EHRCommonOptions? options = null)
    {
        options ??= new EHRCommonOptions();

        // Load from configuration if provided
        configuration.GetSection("EHRCommon").Bind(options);

        if (options.EnableLogging)
        {
            services.AddSerilogLogging();
        }

        if (options.EnableCaching)
        {
            services.AddCaching(options);
        }

        if (options.EnableEncryption)
        {
            services.AddEncryption(options);
        }

        // Add MediatR behaviors
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        return services;
    }

    /// <summary>
    /// Add Redis caching services.
    /// </summary>
    private static IServiceCollection AddCaching(
        this IServiceCollection services,
        EHRCommonOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.RedisConnectionString))
            throw new InvalidOperationException(
                "Redis connection string is required. Set EHRCommon:RedisConnectionString in configuration.");

        try
        {
            // Connect to Redis and verify connectivity
            var connectionMultiplexer = ConnectionMultiplexer.Connect(options.RedisConnectionString);

            // Register singleton connection multiplexer
            services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

            // Register cache service
            services.AddSingleton<ICacheService, RedisCacheService>();

            // Add health check
            services.AddHealthChecks()
                .AddCacheHealthCheck();

            return services;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to connect to Redis at {options.RedisConnectionString}", ex);
        }
    }

    /// <summary>
    /// Add encryption and password hashing services.
    /// </summary>
    private static IServiceCollection AddEncryption(
        this IServiceCollection services,
        EHRCommonOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.EncryptionKey))
            throw new InvalidOperationException(
                "Encryption key is required. Set EHRCommon:EncryptionKey in configuration or environment variables.");

        // Register encryption service as singleton (stateless, thread-safe)
        services.AddSingleton<IEncryptionService>(
            new EncryptionService(options.EncryptionKey));

        // Register password hasher as singleton
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }

    /// <summary>
    /// Add Serilog structured logging.
    /// </summary>
    private static IServiceCollection AddSerilogLogging(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                "logs/ehr-platform-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .Enrich.FromLogContext()
            .CreateLogger();

        services.AddLogging(logBuilder =>
        {
            logBuilder.ClearProviders();
            logBuilder.AddSerilog();
        });

        return services;
    }
