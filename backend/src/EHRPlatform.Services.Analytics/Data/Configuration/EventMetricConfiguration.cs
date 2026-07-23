using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Analytics.Domain.Entities;

namespace EHRPlatform.Services.Analytics.Data.Configuration;

/// <summary>
/// Entity configuration for EventMetric.
/// </summary>
public class EventMetricConfiguration : IEntityTypeConfiguration<EventMetric>
{
    public void Configure(EntityTypeBuilder<EventMetric> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.EventType);
        entity.HasIndex(e => e.OccurredAt).IsDescending();
        entity.Property(e => e.EventType).IsRequired().HasMaxLength(100);
    }
}
