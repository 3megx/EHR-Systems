using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Identity.Domain.Entities;

namespace EHRPlatform.Services.Identity.Data.Configuration;

/// <summary>
/// Entity configuration for RefreshToken.
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Token).IsUnique();
        entity.HasOne(e => e.User).WithMany(u => u.RefreshTokens).HasForeignKey(e => e.UserId);
        entity.Property(e => e.Token).IsRequired();
        entity.Property(e => e.ExpiresAt).IsRequired();
    }
}
