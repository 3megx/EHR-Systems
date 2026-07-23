using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Billing.Domain;

namespace EHRPlatform.Services.Billing;

/// <summary>
/// DbContext for Billing Service.
/// </summary>
public class BillingContext : BaseDbContext
{
    public BillingContext(DbContextOptions<BillingContext> options) : base(options) { }

    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<LineItem> LineItems { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<InsuranceClaim> InsuranceClaims { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ServiceDate).IsDescending();
            entity.HasIndex(e => e.DueDate);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<LineItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Invoice).WithMany(i => i.LineItems).HasForeignKey(e => e.InvoiceId);
            entity.HasIndex(e => e.InvoiceId);
            entity.Property(e => e.CPTCode).HasMaxLength(10);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Invoice).WithMany(i => i.Payments).HasForeignKey(e => e.InvoiceId);
            entity.HasIndex(e => e.InvoiceId);
            entity.HasIndex(e => e.ReceivedAt).IsDescending();
            entity.Property(e => e.Method).HasMaxLength(50);
        });

        modelBuilder.Entity<InsuranceClaim>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Invoice).WithMany(i => i.InsuranceClaims).HasForeignKey(e => e.InvoiceId);
            entity.HasIndex(e => e.InvoiceId);
            entity.HasIndex(e => e.ClaimNumber).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.ClaimNumber).HasMaxLength(50);
        });
    }
}
