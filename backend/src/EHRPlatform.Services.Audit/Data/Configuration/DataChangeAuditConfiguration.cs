using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Audit.Domain.Entities;

namespace EHRPlatform.Services.Audit.Data.Configuration;

/// <summary>
/// Entity configuration for DataChangeAudit.
/// </summary>
public class DataChangeAuditConfiguration : IEntityTypeConfiguration<DataChangeAudit>
{
    public void Configure(EntityTypeBuilder<DataChangeAudit> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => new { e.ResourceType, e.ResourceId });
        entity.HasIndex(e => e.FieldName);
        entity.HasIndex(e => e.ChangedAt).IsDescending();
        entity.Property(e => e.ResourceType).HasMaxLength(100);
        entity.Property(e => e.FieldName).HasMaxLength(100);
    }
}
