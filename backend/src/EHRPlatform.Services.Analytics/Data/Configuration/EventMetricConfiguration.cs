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
        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.EventType);
        entity.HasIndex(x => x.OccurredAt).IsDescending();
    }
}
