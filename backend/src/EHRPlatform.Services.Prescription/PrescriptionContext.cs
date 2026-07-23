using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Prescription.Features.Prescriptions.Domain;

namespace EHRPlatform.Services.Prescription;

/// <summary>
/// DbContext for Prescription Service.
/// </summary>
public class PrescriptionContext : BaseDbContext
{
    public PrescriptionContext(DbContextOptions<PrescriptionContext> options) : base(options) { }

    public DbSet<Domain.Prescription> Prescriptions { get; set; } = null!;
    public DbSet<PrescriptionRefill> PrescriptionRefills { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Prescription configuration
        modelBuilder.Entity<Domain.Prescription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.ProviderId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartDate).IsDescending();
            entity.Property(e => e.MedicationName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Active");
            entity.Property(e => e.FormType).HasMaxLength(50);
        });

        // PrescriptionRefill configuration
        modelBuilder.Entity<PrescriptionRefill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Prescription)
                .WithMany(p => p.Refills)
                .HasForeignKey(e => e.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.PrescriptionId);
            entity.HasIndex(e => new { e.PrescriptionId, e.Status });
            entity.Property(e => e.Status).HasMaxLength(50);
        });
    }
}
