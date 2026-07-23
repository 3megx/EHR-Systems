using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EHRPlatform.Services.Billing.Data.Configuration;

/// <summary>
/// Entity configuration for LineItem.
/// </summary>
public class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
{
    public void Configure(EntityTypeBuilder<LineItem> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasOne(e => e.Invoice)
            .WithMany(i => i.LineItems)
            .HasForeignKey(e => e.InvoiceId);
        entity.HasIndex(e => e.InvoiceId);
        entity.Property(e => e.CPTCode).HasMaxLength(10);
    }
}
