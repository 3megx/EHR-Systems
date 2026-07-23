using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EHRPlatform.Services.Billing.Data.Configuration;

/// <summary>
/// Entity configuration for Invoice aggregate.
/// </summary>
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.PatientId);
        entity.HasIndex(e => e.InvoiceNumber).IsUnique();
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.ServiceDate).IsDescending();
        entity.HasIndex(e => e.DueDate);
        entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
    }
}
