using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Analytics.Domain.Entities;

namespace EHRPlatform.Services.Analytics.Data.Configuration;

/// <summary>
/// Entity configuration for AnalyticsMetric.
/// </summary>
public class AnalyticsMetricConfiguration : IEntityTypeConfiguration<AnalyticsMetric>
{
    public void Configure(EntityTypeBuilder<AnalyticsMetric> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.MetricName);
        entity.HasIndex(e => e.Category);
        entity.HasIndex(e => new { e.PeriodStart, e.PeriodEnd });
        entity.Property(e => e.MetricName).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Category).HasMaxLength(100);
    }
}
