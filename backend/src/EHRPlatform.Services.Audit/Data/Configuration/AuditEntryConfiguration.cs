using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Audit.Domain.Entities;

namespace EHRPlatform.Services.Audit.Data.Configuration;

/// <summary>
/// Entity configuration for AuditEntry.
/// </summary>
public class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => new { e.ResourceType, e.ResourceId });
        entity.HasIndex(e => e.Timestamp).IsDescending();
        entity.HasIndex(e => e.Action);
        entity.HasIndex(e => e.Status);
        entity.Property(e => e.IntegrityHash).IsRequired();
        entity.Property(e => e.UserEmail).HasMaxLength(255);
        entity.Property(e => e.Action).HasMaxLength(50);
        entity.Property(e => e.Status).HasMaxLength(50);
    }
}
