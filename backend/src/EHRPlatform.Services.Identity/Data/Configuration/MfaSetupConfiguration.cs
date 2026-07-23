using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Identity.Domain.Entities;

namespace EHRPlatform.Services.Identity.Data.Configuration;

/// <summary>
/// Entity configuration for MfaSetup.
/// </summary>
public class MfaSetupConfiguration : IEntityTypeConfiguration<MfaSetup>
{
    public void Configure(EntityTypeBuilder<MfaSetup> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasOne(e => e.User).WithMany(u => u.MfaSetups).HasForeignKey(e => e.UserId);
    }
}
