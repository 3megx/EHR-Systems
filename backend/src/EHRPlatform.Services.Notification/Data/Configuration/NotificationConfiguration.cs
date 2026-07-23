using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Notification.Domain.Entities;

namespace EHRPlatform.Services.Notification.Data.Configuration;

/// <summary>
/// Entity configuration for Notification.
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.RecipientId);
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.CreatedAt).IsDescending();
        entity.Property(e => e.Channel).IsRequired().HasMaxLength(50);
        entity.Property(e => e.NotificationType).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
        entity.Property(e => e.Subject).HasMaxLength(500);
    }
}
