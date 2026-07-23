using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Audit.Domain.Entities;

namespace EHRPlatform.Services.Audit.Data.Configuration;

/// <summary>
/// Entity configuration for DataChangeAudit (append-only).
/// </summary>
public class DataChangeAuditConfiguration : IEntityTypeConfiguration<DataChangeAudit>
{
    public void Configure(EntityTypeBuilder<DataChangeAudit> entity)
    {
        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => new { x.ResourceType, x.ResourceId });
        entity.HasIndex(x => x.FieldName);
        entity.HasIndex(x => x.ChangedAt).IsDescending();
    }
}
