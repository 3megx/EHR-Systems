using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Notification.Features.Notifications.Domain;

namespace EHRPlatform.Services.Notification.Data;

/// <summary>
/// DbContext for Notification Service.
/// Single Responsibility: Configure entity mappings and relationships.
/// </summary>
public class NotificationContext : BaseDbContext
{
    public NotificationContext(DbContextOptions<NotificationContext> options) : base(options) { }

    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; } = null!;
    public DbSet<NotificationPreference> NotificationPreferences { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Notification entity
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.RecipientId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Channel);
            entity.HasIndex(e => e.CreatedAt).IsDescending();
            entity.HasIndex(e => e.ScheduledFor);
            entity.Property(e => e.Channel).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        // Configure NotificationTemplate entity
        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Channel).HasMaxLength(50);
            entity.Property(e => e.Subject).HasMaxLength(255);
        });

        // Configure NotificationPreference entity
        modelBuilder.Entity<NotificationPreference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.Channel, e.NotificationType }).IsUnique();
            entity.Property(e => e.Channel).HasMaxLength(50);
        });
    }
}
