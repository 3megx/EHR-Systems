using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Audit.Domain.Entities;

namespace EHRPlatform.Services.Audit.Data.Configuration;

/// <summary>
/// Entity configuration for AuditLogExport.
/// </summary>
public class AuditLogExportConfiguration : IEntityTypeConfiguration<AuditLogExport>
{
    public void Configure(EntityTypeBuilder<AuditLogExport> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.ExportedAt).IsDescending();
        entity.HasIndex(e => e.Status);
        entity.Property(e => e.FileHash).IsRequired();
        entity.Property(e => e.Status).HasMaxLength(50);
        entity.Property(e => e.ExportedBy).HasMaxLength(255);
    }
}
