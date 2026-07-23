using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Notification.Domain.Entities;

namespace EHRPlatform.Services.Notification.Data.Configuration;

/// <summary>
/// Entity configuration for NotificationPreference.
/// </summary>
public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.RecipientId);
        entity.Property(e => e.Channel).IsRequired().HasMaxLength(50);
        entity.Property(e => e.NotificationType).IsRequired().HasMaxLength(50);
        entity.Property(e => e.IsEnabled).HasDefaultValue(true);
    }
}
