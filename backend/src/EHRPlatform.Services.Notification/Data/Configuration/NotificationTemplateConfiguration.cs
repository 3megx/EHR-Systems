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
        entity.HasIndex(e => e.TemplateCode).IsUnique();
        entity.Property(e => e.TemplateCode).IsRequired().HasMaxLength(100);
        entity.Property(e => e.TemplateName).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Channel).IsRequired().HasMaxLength(50);
    }
}
