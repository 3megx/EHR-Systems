using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Identity.Domain.Entities;

namespace EHRPlatform.Services.Identity.Data;

/// <summary>
/// DbContext for Identity Service.
/// Manages users, roles, permissions, tokens, audit.
/// </summary>
public class IdentityContext : BaseDbContext
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<LoginAudit> LoginAudits { get; set; } = null!;
    public DbSet<MfaSetup> MfaSetups { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
        SeedDefaultRoles(modelBuilder);
    }

    private void SeedDefaultRoles(ModelBuilder modelBuilder)
    {
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var doctorRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var nurseRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var patientRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "Admin", Description = "System administrator" },
            new Role { Id = doctorRoleId, Name = "Doctor", Description = "Healthcare provider" },
            new Role { Id = nurseRoleId, Name = "Nurse", Description = "Nursing staff" },
            new Role { Id = patientRoleId, Name = "Patient", Description = "Patient user" }
        );
    }
}
