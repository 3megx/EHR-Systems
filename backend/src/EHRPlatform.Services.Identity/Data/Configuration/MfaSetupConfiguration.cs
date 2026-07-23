using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Identity.Domain.Entities;

namespace EHRPlatform.Services.Identity.Data.Configuration;

/// <summary>
/// Entity configuration for MfaSetup (Multi-Factor Authentication).
/// </summary>
public class MfaSetupConfiguration : IEntityTypeConfiguration<MfaSetup>
{
    public void Configure(EntityTypeBuilder<MfaSetup> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        entity.Property(e => e.MfaType).IsRequired().HasMaxLength(50);
        entity.Property(e => e.IsEnabled).HasDefaultValue(false);
    }
}
