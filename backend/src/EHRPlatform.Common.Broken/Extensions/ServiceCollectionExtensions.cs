namespace EHRPlatform.Common.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;
using MediatR;
using Mapster;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

/// <summary>
/// Extension methods for configuring common services in the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds common EHR services and infrastructure.
    /// </summary>
    public static IServiceCollection AddEHRCommon(
        this IServiceCollection services,
        Action<EHRCommonOptions>? configure = null)
    {
        var options = new EHRCommonOptions();
        configure?.Invoke(options);

        // Register options
        services.AddSingleton(options);

        // Add Mapster with auto-discovery of mapping profiles
        services.AddMapster();
        services.AddMapsterProfiles(AppDomain.CurrentDomain.GetAssemblies());
        services.AddServiceMappers(AppDomain.CurrentDomain.GetAssemblies());

        // Add validation
        services.AddValidation();

        // Add caching
        if (options.EnableCaching)
        {
            services.AddCaching(options);
        }

        // Add search (Elasticsearch)
        if (options.EnableSearch)
        {
            services.AddSearch(options);
        }

        // Add messaging
        if (options.EnableMessaging)
        {
            services.AddMessaging(options);
        }

        // Add security & encryption
        services.AddSecurity(options);

        return services;
    }

    /// <summary>
    /// Adds CQRS infrastructure with MediatR and all pipeline behaviors.
    /// Configures validation, logging, caching, and transaction support.
    /// </summary>
    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<ServiceCollectionExtensions>();
        });

        // Add CQRS behaviors
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds validation pipeline behaviors to MediatR.
    /// </summary>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        // Register all validators from assemblies
        services.AddValidatorsFromAssemblyContaining<ServiceCollectionExtensions>();

        // Add validation behavior to MediatR pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds distributed caching with Redis.
    /// </summary>
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        EHRCommonOptions options)
    {
        if (string.IsNullOrEmpty(options.RedisConnectionString))
            throw new ArgumentNullException(nameof(options.RedisConnectionString));

        // Add Redis distributed cache
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            redisOptions.Configuration = options.RedisConnectionString;
        });

        // Add cache service
        services.AddSingleton<ICacheService, RedisCacheService>();

        // Add caching behavior to MediatR pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds Elasticsearch integration for search functionality.
    /// </summary>
    public static IServiceCollection AddSearch(
        this IServiceCollection services,
        EHRCommonOptions options)
    {
        if (string.IsNullOrEmpty(options.ElasticsearchUrl))
            throw new ArgumentNullException(nameof(options.ElasticsearchUrl));

        // Add Elasticsearch client
        var settings = new Elastic.Clients.Elasticsearch.ElasticsearchClientSettings(
            new Uri(options.ElasticsearchUrl))
            .Authentication(new BasicAuthentication(options.ElasticsearchUsername, options.ElasticsearchPassword));

        var client = new Elastic.Clients.Elasticsearch.ElasticsearchClient(settings);
        services.AddSingleton(client);

        // Add search service
        services.AddSingleton<ISearchService, ElasticsearchService>();

        return services;
    }

    /// <summary>
    /// Adds Kafka messaging infrastructure.
    /// </summary>
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        EHRCommonOptions options)
    {
        if (string.IsNullOrEmpty(options.KafkaBootstrapServers))
            throw new ArgumentNullException(nameof(options.KafkaBootstrapServers));

        // Add event publisher
        services.AddSingleton<IEventPublisher, KafkaEventPublisher>();

        // Add outbox processor
        services.AddHostedService<OutboxProcessor>();

        return services;
    }

    /// <summary>
    /// Adds security and encryption services.
    /// </summary>
    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        EHRCommonOptions options)
    {
        if (string.IsNullOrEmpty(options.EncryptionKey))
            throw new ArgumentNullException(nameof(options.EncryptionKey));

        // Add encryption service
        services.AddSingleton<IEncryptionService>(sp =>
            new EncryptionService(options.EncryptionKey));

        // Add password hasher
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }

    /// <summary>
    /// Adds Serilog structured logging with proper configuration.
    /// </summary>
    public static IServiceCollection AddSerilogLogging(
        this IServiceCollection services,
        Action<LoggerConfiguration>? configure = null)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", "EHRPlatform")
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentUserName();

        // Add console sink
        loggerConfig.WriteTo.Console(new CompactJsonFormatter());

        // Add file sink with rolling files
        loggerConfig.WriteTo.File(
            "logs/.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

        // Allow custom configuration
        configure?.Invoke(loggerConfig);

        Log.Logger = loggerConfig.CreateLogger();

        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));

        return services;
    }
}

/// <summary>
/// Options for configuring EHR common services.
/// </summary>
public class EHRCommonOptions
{
    /// <summary>
    /// Enable distributed caching with Redis.
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Redis connection string.
    /// </summary>
    public string? RedisConnectionString { get; set; }

    /// <summary>
    /// Enable Elasticsearch search functionality.
    /// </summary>
    public bool EnableSearch { get; set; } = true;

    /// <summary>
    /// Elasticsearch URL.
    /// </summary>
    public string? ElasticsearchUrl { get; set; }

    /// <summary>
    /// Elasticsearch username.
    /// </summary>
    public string? ElasticsearchUsername { get; set; }

    /// <summary>
    /// Elasticsearch password.
    /// </summary>
    public string? ElasticsearchPassword { get; set; }

    /// <summary>
    /// Enable Kafka messaging.
    /// </summary>
    public bool EnableMessaging { get; set; } = true;

    /// <summary>
    /// Kafka bootstrap servers.
    /// </summary>
    public string? KafkaBootstrapServers { get; set; }

    /// <summary>
    /// Encryption key for sensitive data.
    /// </summary>
    public string? EncryptionKey { get; set; }

    /// <summary>
    /// Enable audit trail logging.
    /// </summary>
    public bool EnableAudit { get; set; } = true;

    /// <summary>
    /// Enable request/response logging.
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>
    /// Enable rate limiting.
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;

    /// <summary>
    /// Default cache duration in seconds.
    /// </summary>
    public int DefaultCacheDurationSeconds { get; set; } = 300;
}
