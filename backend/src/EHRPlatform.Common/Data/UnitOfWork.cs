#nullable enable

using EHRPlatform.Common.Entities;
using EHRPlatform.Common.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EHRPlatform.Common.Data;

/// <summary>
/// Unit of Work implementation for managing repositories and transactions.
/// Uses repository factory pattern to create repositories on-demand.
/// Manages transaction lifecycle and ensures ACID compliance.
/// HIPAA compliant: all changes are audited before commit.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public DbContext DbContext => _context;

    public UnitOfWork(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Get or create repository for entity type using factory pattern.
    /// Repositories are cached to ensure single instance per entity type per UnitOfWork.
    /// </summary>
    public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity);
        
        if (_repositories.TryGetValue(type, out var repo))
        {
            return (IRepository<TEntity>)repo;
        }

        // Create new repository for this entity type
        var repositoryType = typeof(Repository<>).MakeGenericType(type);
        var newRepository = Activator.CreateInstance(repositoryType, _context)
            ?? throw new InvalidOperationException($"Failed to create repository for {type.Name}");

        _repositories[type] = newRepository;
        return (IRepository<TEntity>)newRepository;
    }

    /// <summary>
    /// Begin a new transaction.
    /// Returns existing transaction if one is already active.
    /// </summary>
    public async Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            // Return existing transaction - only one allowed
            return _transaction;
        }

        try
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return _transaction;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to begin database transaction", ex);
        }
    }

    /// <summary>
    /// Commit the active transaction.
    /// All changes are persisted to database.
    /// Transaction is disposed after commit.
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Attempt rollback on commit failure
            if (_transaction != null)
            {
                await RollbackTransactionAsync(cancellationToken);
            }
            throw new InvalidOperationException("Failed to commit database transaction", ex);
        }
    }

    /// <summary>
    /// Rollback the active transaction.
    /// All changes since transaction began are discarded.
    /// Transaction is disposed after rollback.
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to rollback database transaction", ex);
        }
    }

    /// <summary>
    /// Save all pending changes to database.
    /// Changes are audited via interceptors before commit.
    /// Returns count of entities modified.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // SaveChangesAsync triggers interceptors which set audit fields
            var changes = await _context.SaveChangesAsync(cancellationToken);
            return changes;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Concurrency conflict occurred while saving changes", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error occurred while saving changes to database", ex);
        }
    }

    /// <summary>
    /// Save changes and publish domain events to outbox.
    /// All operations occur within same transaction - ensures consistency.
    /// 
    /// Process:
    /// 1. Collect domain events from all aggregates
    /// 2. Save entity changes to database
    /// 3. Convert domain events to integration events
    /// 4. Insert integration events into outbox table
    /// 5. Commit transaction (all-or-nothing)
    /// 
    /// BackgroundService processes outbox asynchronously and publishes to Kafka.
    /// This ensures guaranteed event delivery even if service crashes.
    /// </summary>
    public async Task<(int changesCount, int eventsCount)> SaveChangesWithEventPublishingAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get all domain events from entities
            var domainEvents = _context.ChangeTracker
                .Entries<BaseEntity>()
                .SelectMany(e => e.Entity.GetDomainEvents())
                .ToList();

            // Clear domain events from entities
            foreach (var entity in _context.ChangeTracker.Entries<BaseEntity>())
            {
                entity.Entity.ClearDomainEvents();
            }

            // Save entity changes
            var changesCount = await _context.SaveChangesAsync(cancellationToken);

            // Convert domain events to integration events and add to outbox
            var integrationEvents = domainEvents
                .Select(de => new OutboxEvent
                {
                    Id = Guid.NewGuid(),
                    EventType = de.GetType().Name,
                    EventData = System.Text.Json.JsonSerializer.Serialize(de),
                    CreatedAt = DateTime.UtcNow,
                    IsPublished = false,
                    PublishedAt = null,
                    PublishAttempts = 0,
                    ErrorMessage = null
                })
                .ToList();

            // Insert outbox events
            if (integrationEvents.Any())
            {
                var outboxDbSet = _context.Set<OutboxEvent>();
                await outboxDbSet.AddRangeAsync(integrationEvents, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return (changesCount, integrationEvents.Count);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error occurred while saving changes and publishing events", ex);
        }
    }

    /// <summary>
    /// Check if there are pending changes not yet saved.
    /// </summary>
    public bool HasPendingChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }

    /// <summary>
    /// Execute action within transaction scope.
    /// Automatically commits on success, rolls back on error.
    /// </summary>
    public async Task ExecuteInTransactionAsync(
        Func<IUnitOfWork, Task> action,
        CancellationToken cancellationToken = default)
    {
        var transaction = await BeginTransactionAsync(cancellationToken);
        
        try
        {
            await action(this);
            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Execute action with transaction and return result.
    /// </summary>
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<IUnitOfWork, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        var transaction = await BeginTransactionAsync(cancellationToken);
        
        try
        {
            var result = await action(this);
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Dispose DbContext and clean up resources.
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }

    /// <summary>
    /// Async disposal of resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        
        await _context.DisposeAsync();
    }
}
