using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EHRPlatform.Services.Appointment.Data.Configuration;

public class ProviderAvailabilityConfiguration : IEntityTypeConfiguration<ProviderAvailability>
{
    public void Configure(EntityTypeBuilder<ProviderAvailability> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.ProviderId);
        entity.HasIndex(e => new { e.ProviderId, e.SlotStart, e.SlotEnd });
        entity.Property(e => e.RecurrencePattern).HasMaxLength(50);
    }
}
