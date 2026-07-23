using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Clinical.Domain.Entities;

namespace EHRPlatform.Services.Clinical.Data.Configuration;

/// <summary>
/// Entity configuration for ClinicalNote.
/// </summary>
public class ClinicalNoteConfiguration : IEntityTypeConfiguration<ClinicalNote>
{
    public void Configure(EntityTypeBuilder<ClinicalNote> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.PatientId);
        entity.HasIndex(e => e.ProviderId);
        entity.HasIndex(e => e.EncounterDate).IsDescending();
        entity.HasIndex(e => e.Status);
        entity.Property(e => e.EncounterType).HasMaxLength(50);
        entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Draft");
        entity.HasMany(e => e.VitalSigns).WithOne(v => v.ClinicalNote).HasForeignKey(v => v.ClinicalNoteId);
        entity.HasMany(e => e.Diagnoses).WithOne(d => d.ClinicalNote).HasForeignKey(d => d.ClinicalNoteId);
        entity.HasMany(e => e.Procedures).WithOne(p => p.ClinicalNote).HasForeignKey(p => p.ClinicalNoteId);
    }
}
