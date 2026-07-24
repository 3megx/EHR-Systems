using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Events;
using EHRPlatform.Services.Identity.Domain.Entities;
using EHRPlatform.Services.Identity.Domain.Enums;

namespace EHRPlatform.Services.Identity.Data;

/// <summary>
/// DbContext for Identity Service.
/// Manages users, roles, permissions, tokens, MFA, and audit records.
///
/// Seeding strategy:
/// - Static (HasData): core roles only — minimal, deterministic, migration-safe.
/// - Runtime: admin user + permissions are seeded by <c>IdentityRuntimeSeeder</c>
///   after <c>EnsureCreatedAsync</c> so that secrets (password hashes) are
///   computed at startup rather than baked into migrations.
/// </summary>
public class IdentityContext : BaseDbContext
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    public DbSet<OutboxEvent>    OutboxEvents    { get; set; } = null!;
    public DbSet<User>           Users           { get; set; } = null!;
    public DbSet<Role>           Roles           { get; set; } = null!;
    public DbSet<Permission>     Permissions     { get; set; } = null!;
    public DbSet<UserRole>       UserRoles       { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<RefreshToken>   RefreshTokens   { get; set; } = null!;
    public DbSet<LoginAudit>     LoginAudits     { get; set; } = null!;
    public DbSet<MfaSetup>       MfaSetups       { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
        SeedCoreRoles(modelBuilder);
    }

    /// <summary>
    /// Static seed data for the seven canonical roles.
    /// Uses fixed, stable GUIDs so re-running EnsureCreatedAsync is safe.
    /// Additional roles are never inserted here — add to <see cref="RoleType"/> and
    /// the runtime seeder picks them up automatically.
    /// </summary>
    private static void SeedCoreRoles(ModelBuilder modelBuilder)
    {
        // Stable GUIDs: index matches (int)RoleType value for easy reference.
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.Parse("10000001-0000-0000-0000-000000000001"), Name = nameof(RoleType.Admin),        Description = "System administrator with full access" },
            new Role { Id = Guid.Parse("10000001-0000-0000-0000-000000000002"), Name = nameof(RoleType.Doctor),       Description = "Licensed healthcare provider" },
            new Role { Id = Guid.Parse("10000001-0000-0000-0000-000000000003"), Name = nameof(RoleType.Nurse),        Description = "Nursing and care staff" },
            new Role { Id = Guid.Parse("10000001-0000-0000-0000-000000000004"), Name = nameof(RoleType.Patient),      Description = "Registered patient account" },
            new Role { Id = Guid.Parse("10000001-0000-0000-0000-000000000005"), Name = nameof(RoleType.Receptionist), Description = "Front-desk and scheduling staff" },
            new Role { Id = Guid.Parse("10000001-0000-0000-0000-000000000006"), Name = nameof(RoleType.Pharmacist),   Description = "Pharmacy and medication management" },
            new Role { Id = Guid.Parse("10000001-0000-0000-0000-000000000007"), Name = nameof(RoleType.Billing),      Description = "Billing and insurance claims" }
        );
    }
}
