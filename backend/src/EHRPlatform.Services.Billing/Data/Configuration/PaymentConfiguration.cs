using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EHRPlatform.Services.Billing.Data.Configuration;

/// <summary>
/// Entity configuration for Payment.
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasOne(e => e.Invoice)
            .WithMany(i => i.Payments)
            .HasForeignKey(e => e.InvoiceId);
        entity.HasIndex(e => e.InvoiceId);
        entity.HasIndex(e => e.ReceivedAt).IsDescending();
        entity.Property(e => e.Method).HasMaxLength(50);
    }
}
