using Microsoft.EntityFrameworkCore;
using EHRPlatform.Services.Identity.Domain.Entities;

namespace EHRPlatform.Services.Identity.Data.Seeds;

/// <summary>
/// Seed data for Identity (Users, Roles, Permissions).
/// </summary>
public static class IdentitySeed
{
    public static void SeedIdentity(this ModelBuilder modelBuilder)
    {
        var adminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var adminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var doctorRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var patientRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRoleId,
                Name = "Admin",
                Description = "System administrator with full access"
            },
            new Role
            {
                Id = doctorRoleId,
                Name = "Doctor",
                Description = "Healthcare provider"
            },
            new Role
            {
                Id = patientRoleId,
                Name = "Patient",
                Description = "Patient user"
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                Email = "admin@ehrs.local",
                FirstName = "Admin",
                LastName = "User",
                PasswordHash = "hashed_password_admin",
                PasswordSalt = "salt_admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            }
        );
    }
}
