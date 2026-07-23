using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Patient.Features.Patients.Domain;

namespace EHRPlatform.Services.Patient;

/// <summary>
/// DbContext for Patient Service.
/// Manages patients, allergies, conditions.
/// </summary>
public class PatientContext : BaseDbContext
{
    public PatientContext(DbContextOptions<PatientContext> options) : base(options) { }

    public DbSet<Domain.Patient> Patients { get; set; } = null!;
    public DbSet<PatientAllergy> PatientAllergies { get; set; } = null!;
    public DbSet<PatientCondition> PatientConditions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Patient configuration
        modelBuilder.Entity<Domain.Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MRN).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.MRN).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BloodType).HasMaxLength(10);
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Active");
        });

        // PatientAllergy configuration
        modelBuilder.Entity<PatientAllergy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Allergies)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.PatientId);
            entity.Property(e => e.Allergen).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Severity).HasMaxLength(50);
        });

        // PatientCondition configuration
        modelBuilder.Entity<PatientCondition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Conditions)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.PatientId);
            entity.Property(e => e.Condition).IsRequired();
            entity.Property(e => e.ICD10Code).IsRequired().HasMaxLength(10);
        });
    }
}
