using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EHRPlatform.Services.Billing.Data.Configuration;

/// <summary>
/// Entity configuration for InsuranceClaim.
/// </summary>
public class InsuranceClaimConfiguration : IEntityTypeConfiguration<InsuranceClaim>
{
    public void Configure(EntityTypeBuilder<InsuranceClaim> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasOne(e => e.Invoice)
            .WithMany(i => i.InsuranceClaims)
            .HasForeignKey(e => e.InvoiceId);
        entity.HasIndex(e => e.InvoiceId);
        entity.HasIndex(e => e.ClaimNumber).IsUnique();
        entity.HasIndex(e => e.Status);
        entity.Property(e => e.ClaimNumber).HasMaxLength(50);
    }
}
