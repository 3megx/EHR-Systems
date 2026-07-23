using Microsoft.EntityFrameworkCore;
using EHRPlatform.Services.Audit.Domain.Entities;

namespace EHRPlatform.Services.Audit.Data.Seeds;

/// <summary>
/// Seed data for Audit (Audit entries, Access logs, Compliance reports).
/// </summary>
public static class AuditSeed
{
    public static void SeedAudit(this ModelBuilder modelBuilder)
    {
        var auditEntryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        modelBuilder.Entity<AuditEntry>().HasData(
            new AuditEntry
            {
                Id = auditEntryId,
                UserId = userId,
                UserEmail = "admin@ehrs.local",
                Action = "LOGIN",
                ResourceType = "User",
                ResourceId = userId.ToString(),
                Status = "Success",
                Timestamp = DateTime.UtcNow,
                IntegrityHash = "hash_value",
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<AccessLog>().HasData(
            new AccessLog
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                UserId = userId,
                UserEmail = "admin@ehrs.local",
                ResourceType = "Patient",
                ResourceId = Guid.Parse("44444444-4444-4444-4444-444444444444").ToString(),
                AccessedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
