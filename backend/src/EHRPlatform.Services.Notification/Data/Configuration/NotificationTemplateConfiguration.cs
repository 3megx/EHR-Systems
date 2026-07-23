using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Notification.Domain.Entities;

namespace EHRPlatform.Services.Notification.Data.Configuration;

/// <summary>
/// Entity configuration for NotificationTemplate.
/// </summary>
public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Name).IsUnique();
        entity.Property(e => e.Channel).HasMaxLength(50);
        entity.Property(e => e.Subject).HasMaxLength(255);
    }
}
