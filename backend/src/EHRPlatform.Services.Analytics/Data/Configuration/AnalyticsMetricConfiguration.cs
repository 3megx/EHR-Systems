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
        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.MetricName);
        entity.HasIndex(x => x.Category);
        entity.HasIndex(x => new { x.PeriodStart, x.PeriodEnd });
    }
}
