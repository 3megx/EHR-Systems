using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Identity.Domain.Entities;

namespace EHRPlatform.Services.Identity.Data.Configuration;

/// <summary>
/// Entity configuration for LoginAudit.
/// </summary>
public class LoginAuditConfiguration : IEntityTypeConfiguration<LoginAudit>
{
    public void Configure(EntityTypeBuilder<LoginAudit> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => new { e.UserId, e.CreatedAt });
        entity.Property(e => e.Email).HasMaxLength(255);
        entity.Property(e => e.Status).HasMaxLength(50);
    }
}
