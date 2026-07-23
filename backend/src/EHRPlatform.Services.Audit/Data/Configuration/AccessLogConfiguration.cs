using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Audit.Domain.Entities;

namespace EHRPlatform.Services.Audit.Data.Configuration;

/// <summary>
/// Entity configuration for AccessLog.
/// </summary>
public class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    public void Configure(EntityTypeBuilder<AccessLog> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => new { e.ResourceType, e.ResourceId });
        entity.HasIndex(e => e.AccessedAt).IsDescending();
        entity.Property(e => e.UserEmail).HasMaxLength(255);
        entity.Property(e => e.ResourceType).HasMaxLength(100);
    }
}
