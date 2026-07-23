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
        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.ExportedAt).IsDescending();
        entity.HasIndex(x => x.Status);
        entity.Property(x => x.FileHash).IsRequired();
    }
}
