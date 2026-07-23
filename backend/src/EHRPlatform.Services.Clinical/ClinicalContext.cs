using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Domain;

namespace EHRPlatform.Services.Clinical;

/// <summary>
/// DbContext for Clinical Service.
/// Manages clinical notes, vitals, diagnoses, procedures.
/// </summary>
public class ClinicalContext : BaseDbContext
{
    public ClinicalContext(DbContextOptions<ClinicalContext> options) : base(options) { }

    public DbSet<ClinicalNote> ClinicalNotes { get; set; } = null!;
    public DbSet<VitalSigns> VitalSigns { get; set; } = null!;
    public DbSet<ClinicalDiagnosis> ClinicalDiagnoses { get; set; } = null!;
    public DbSet<ClinicalProcedure> ClinicalProcedures { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ClinicalNote configuration
        modelBuilder.Entity<ClinicalNote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.ProviderId);
            entity.HasIndex(e => e.EncounterDate).IsDescending();
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Draft");
            entity.Property(e => e.EncounterType).HasMaxLength(50);
        });

        // VitalSigns configuration
        modelBuilder.Entity<VitalSigns>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.ClinicalNote)
                .WithMany(n => n.VitalSigns)
                .HasForeignKey(e => e.ClinicalNoteId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.ClinicalNoteId);
            entity.HasIndex(e => e.RecordedAt).IsDescending();
        });

        // ClinicalDiagnosis configuration
        modelBuilder.Entity<ClinicalDiagnosis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.ClinicalNote)
                .WithMany(n => n.Diagnoses)
                .HasForeignKey(e => e.ClinicalNoteId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.ClinicalNoteId);
            entity.HasIndex(e => e.DiagnosisCode);
            entity.Property(e => e.DiagnosisCode).HasMaxLength(10);
        });

        // ClinicalProcedure configuration
        modelBuilder.Entity<ClinicalProcedure>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.ClinicalNote)
                .WithMany(n => n.Procedures)
                .HasForeignKey(e => e.ClinicalNoteId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.ClinicalNoteId);
            entity.HasIndex(e => e.PerformedAt).IsDescending();
            entity.Property(e => e.ProcedureCode).HasMaxLength(20);
        });
    }
}
