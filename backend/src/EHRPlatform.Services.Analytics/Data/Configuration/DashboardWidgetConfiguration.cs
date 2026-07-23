using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Analytics.Domain.Entities;

namespace EHRPlatform.Services.Analytics.Data.Configuration;

/// <summary>
/// Entity configuration for DashboardWidget.
/// </summary>
public class DashboardWidgetConfiguration : IEntityTypeConfiguration<DashboardWidget>
{
    public void Configure(EntityTypeBuilder<DashboardWidget> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasOne(e => e.Dashboard).WithMany(d => d.DashboardWidgets).HasForeignKey(e => e.DashboardId);
        entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        entity.Property(e => e.WidgetType).HasMaxLength(50);
    }
}
