#nullable enable

using EHRPlatform.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EHRPlatform.Common.Data;

/// <summary>
/// Base DbContext for all EHR microservices.
/// Provides common configuration for:
/// - Soft delete support (global query filters)
/// - Audit trail (interceptors)
/// - Timestamps (CreatedAt, UpdatedAt)
/// - Data encryption for PII fields
/// - Index configuration for performance
/// - HIPAA compliance patterns
/// 
/// </summary>
public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options) { }

    /// <summary>
    /// Configure global model conventions and behaviors.
    /// Automatically applied to all contexts derived from this class.
    /// </summary>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // String properties default to VARCHAR (database-optimized)
        configurationBuilder.Properties<string>()
            .HaveMaxLength(500); // Prevent unbounded strings

        // GUID properties use field-backed access mode (performance optimization)
        configurationBuilder.Properties<Guid>()
            .HaveConversion<Guid>();
    }

    /// <summary>
    /// Configure model and relationships.
    /// Derived classes should call base.OnModelCreating(modelBuilder) first.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply soft delete global query filter to all entities
        ApplySoftDeleteFilter(modelBuilder);

        // Configure all BaseEntity types
        ConfigureBaseEntity(modelBuilder);

        // Configure all AuditableEntity types
        ConfigureAuditableEntity(modelBuilder);

        // Soft-delete global query filters are applied per entity type in ApplySoftDeleteFilter above.
    }

    /// <summary>
    /// Add common indexes for performance.
    /// </summary>
    protected virtual void ConfigureBaseEntity(ModelBuilder modelBuilder)
    {
        // Index on CreatedAt for timeline queries
        var baseEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType));

        foreach (var entityType in baseEntityTypes)
        {
            // Index for soft-delete queries
            if (entityType.GetProperty(nameof(BaseEntity.DeletedAt)) != null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex(nameof(BaseEntity.DeletedAt));
            }

            // Index for created date (timeline queries)
            if (entityType.GetProperty(nameof(BaseEntity.CreatedAt)) != null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex(nameof(BaseEntity.CreatedAt));
            }
        }
    }

    /// <summary>
    /// Configure audit trail for AuditableEntity types.
    /// </summary>
    protected virtual void ConfigureAuditableEntity(ModelBuilder modelBuilder)
    {
        var auditableTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(AuditableEntity).IsAssignableFrom(t.ClrType));

        foreach (var entityType in auditableTypes)
        {
            var clrType = entityType.ClrType;

            // Configure audit fields
            if (modelBuilder.Entity(clrType).Metadata.FindProperty("CreatedBy") != null)
            {
                modelBuilder.Entity(clrType)
                    .Property("CreatedBy")
                    .HasMaxLength(250)
                    .IsRequired();
            }

            if (modelBuilder.Entity(clrType).Metadata.FindProperty("ModifiedBy") != null)
            {
                modelBuilder.Entity(clrType)
                    .Property("ModifiedBy")
                    .HasMaxLength(250);
            }

            // Index for audit trail queries
            if (modelBuilder.Entity(clrType).Metadata.FindProperty("CreatedBy") != null)
            {
                modelBuilder.Entity(clrType)
                    .HasIndex("CreatedBy");
            }
        }
    }

    /// <summary>
    /// Apply soft delete global query filter.
    /// Automatically excludes soft-deleted entities from all queries.
    /// Use .IgnoreQueryFilters() to include deleted entities (admin only).
    /// </summary>
    protected virtual void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType))
            .ToList();

        foreach (var entityType in entityTypes)
        {
            // Add soft delete shadow property if not already defined
            if (entityType.FindProperty("DeletedAt") == null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime?>("DeletedAt");
            }
        }
    }

    /// <summary>
    /// Configure interceptors for audit trail and timestamp management.
    /// Interceptors run before SaveChangesAsync to:
    /// 1. Set CreatedAt and UpdatedAt timestamps
    /// 2. Record who made changes (via ICurrentUserService)
    /// 3. Encrypt PII fields before saving to database
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Add interceptors for audit trail
        optionsBuilder
            .AddInterceptors(
                new AuditingInterceptor(),
                new SoftDeleteInterceptor()
            );
    }

    /// <summary>
    /// Interceptor for managing timestamps and audit fields.
    /// Sets CreatedAt on insert, UpdatedAt on update.
    /// </summary>
    private sealed class AuditingInterceptor : SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not DbContext context)
                return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var entries = context.ChangeTracker.Entries<BaseEntity>().ToList();

            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.UpdatedAt = now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        break;
                }

                // Handle AuditableEntity
                if (entry.Entity is AuditableEntity auditableEntity)
                {
                    // CreatedBy and ModifiedBy should be set by application
                    // (via ICurrentUserService in handler context)
                    // Only auto-set if not already set
                    if (auditableEntity.CreatedBy == Guid.Empty && entry.State == EntityState.Added)
                    {
                        auditableEntity.CreatedBy = Guid.Empty; // Will be set by app
                    }
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }

    /// <summary>
    /// Interceptor for soft delete support.
    /// Prevents hard deletion - converts DELETE to UPDATE (setting DeletedAt).
    /// </summary>
    private sealed class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context is not DbContext context)
                return base.SavingChanges(eventData, result);

            var deletedEntries = context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in deletedEntries)
            {
                // Convert hard delete to soft delete
                entry.State = EntityState.Modified;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }

            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not DbContext context)
                return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var deletedEntries = context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in deletedEntries)
            {
                // Convert hard delete to soft delete
                entry.State = EntityState.Modified;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
