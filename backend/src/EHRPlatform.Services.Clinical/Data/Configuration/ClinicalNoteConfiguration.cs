using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Domain;

namespace EHRPlatform.Services.Clinical.Data.Configuration;

/// <summary>
/// Entity configuration for ClinicalNote.
/// Single Responsibility: Configure ClinicalNote entity mappings and relationships.
/// </summary>
public class ClinicalNoteConfiguration : IEntityTypeConfiguration<ClinicalNote>
{
    public void Configure(EntityTypeBuilder<ClinicalNote> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.ProviderId);
        builder.HasIndex(e => e.EncounterDate).IsDescending();
        
        builder.Property(e => e.Status)
            .HasMaxLength(50)
            .HasDefaultValue("Draft");
        
        builder.Property(e => e.EncounterType)
            .HasMaxLength(50);

        builder.Property(e => e.Subjective)
            .HasMaxLength(4000);

        builder.Property(e => e.Objective)
            .HasMaxLength(4000);

        builder.Property(e => e.Assessment)
            .HasMaxLength(4000);

        builder.Property(e => e.Plan)
            .HasMaxLength(4000);

        builder.HasMany(e => e.VitalSigns)
            .WithOne(v => v.ClinicalNote)
            .HasForeignKey(v => v.ClinicalNoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Diagnoses)
            .WithOne(d => d.ClinicalNote)
            .HasForeignKey(d => d.ClinicalNoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Procedures)
            .WithOne(p => p.ClinicalNote)
            .HasForeignKey(p => p.ClinicalNoteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
