using EHRPlatform.Common.Data;
using EHRPlatform.Common.Caching;
using EHRPlatform.Common.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Elastic.Clients.Elasticsearch;

namespace EHRPlatform.Common.Extensions;

/// <summary>
/// Dependency injection extensions for data access layer.
/// Configures EF Core, repositories, unit of work, and Redis caching.
/// 
/// Usage in microservice Program.cs:
/// builder.Services
///     .AddPostgresDataAccess<PatientContext>(connectionString)
///     .AddRedisCaching(redisConnectionString);
/// </summary>
public static class DataAccessExtensions
{
    /// <summary>
    /// Register data access services for a microservice.
    /// Configures Entity Framework Core with PostgreSQL.
    /// Registers Unit of Work and repository factory.
    /// </summary>
    /// <typeparam name="TDbContext">DbContext type for microservice (must derive from BaseDbContext)</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">DbContext configuration action</param>
    public static IServiceCollection AddDataAccess<TDbContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureOptions)
        where TDbContext : BaseDbContext
    {
        // Register DbContext with configuration
        services.AddDbContext<TDbContext>(configureOptions);

        // Register Unit of Work (scoped per request)
        services.AddScoped<IUnitOfWork>(provider =>
            new UnitOfWork(provider.GetRequiredService<TDbContext>())
        );

        // Add migration support
        services.AddDatabaseMigrationSupport<TDbContext>();

        return services;
    }

    /// <summary>
    /// Register data access with specific connection string.
    /// Automatically configures PostgreSQL with standard settings.
    /// </summary>
    public static IServiceCollection AddPostgresDataAccess<TDbContext>(
        this IServiceCollection services,
        string? connectionString)
        where TDbContext : BaseDbContext
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Connection string is required", nameof(connectionString));

        return services.AddDataAccess<TDbContext>(options =>
            options.UseNpgsql(connectionString,
                npgsqlOptions =>
                {
                    // Enable case-insensitive searches
                    npgsqlOptions.UseAdminDatabase(false);
                    
                    // Command timeout
                    npgsqlOptions.CommandTimeout(30);
                    
                    // Enable all CITEXT support for case-insensitive columns
                    npgsqlOptions.MapEnum<EncryptionStatus>();
                    npgsqlOptions.MapEnum<AccessLevel>();
                })
                .EnableSensitiveDataLogging(false) // Don't log PII
                .EnableDetailedErrors()
        );
    }

    /// <summary>
    /// Register Redis distributed caching.
    /// Integrates with CQRS caching behavior.
    /// </summary>
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Redis connection string is required", nameof(connectionString));

        // Register Redis connection multiplexer (singleton)
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 5000;
            options.SyncTimeout = 5000;
            return ConnectionMultiplexer.Connect(options);
        });

        // Register cache service (singleton - reuses connection multiplexer)
        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }

    /// <summary>
    /// Add database migration support.
    /// Creates migrator service for applying pending migrations at startup.
    /// </summary>
    private static IServiceCollection AddDatabaseMigrationSupport<TDbContext>(
        this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IDatabaseMigrator, DatabaseMigrator<TDbContext>>();
        return services;
    }
}

/// <summary>
/// Database migration interface for applying pending migrations.
/// </summary>
public interface IDatabaseMigrator
{
    Task MigrateDatabaseAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of database migrator.
/// Applied pending migrations at application startup.
/// </summary>
internal sealed class DatabaseMigrator<TDbContext> : IDatabaseMigrator
    where TDbContext : DbContext
{
    private readonly TDbContext _context;

    public DatabaseMigrator(TDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Apply any pending migrations to database.
    /// Called during application startup via HostedService.
    /// </summary>
    public async Task MigrateDatabaseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Apply all pending migrations
            await _context.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Don't fail startup on migration errors
            // Log and allow manual intervention
            System.Console.WriteLine($"Database migration failed: {ex.Message}");
            throw;
        }
    }
}

/// <summary>
/// Hosted service to run migrations at application startup.
/// </summary>
public class DatabaseMigrationHostedService : Microsoft.Extensions.Hosting.IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseMigrationHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var migrators = scope.ServiceProvider.GetServices<IDatabaseMigrator>();

        foreach (var migrator in migrators)
        {
            await migrator.MigrateDatabaseAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

/// <summary>
/// Status enum for data encryption.
/// </summary>
public enum EncryptionStatus
{
    Encrypted,
    Unencrypted,
    Partial
}

/// <summary>
/// Access level enum for audit trail.
/// </summary>
public enum AccessLevel
{
    None = 0,
    Audit = 1,
    Clinical = 2,
    Administrative = 3,
    Full = 4
}

    /// <summary>
    /// Register Elasticsearch search service.
    /// Enables full-text search on indexed entities.
    /// </summary>
    public static IServiceCollection AddElasticsearchSearch(
        this IServiceCollection services,
        string? elasticsearchUrl)
    {
        if (string.IsNullOrEmpty(elasticsearchUrl))
            throw new ArgumentException("Elasticsearch URL is required", nameof(elasticsearchUrl));

        // Register Elasticsearch client
        var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
            .DisableDirectStreaming()
            .ThrowExceptions();

        var client = new ElasticsearchClient(settings);
        services.AddSingleton(client);

        // Register search service
        services.AddSingleton<ISearchService, ElasticsearchService>();

        return services;
    }
