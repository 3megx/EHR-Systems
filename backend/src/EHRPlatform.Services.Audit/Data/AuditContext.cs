using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Audit.Domain.Entities;

namespace EHRPlatform.Services.Audit.Data;

/// <summary>
/// DbContext for Audit Service.
/// Manages audit logs, access logs, compliance reports (HIPAA-compliant).
/// </summary>
public class AuditContext : BaseDbContext
{
    public AuditContext(DbContextOptions<AuditContext> options) : base(options) { }

    public DbSet<AuditEntry> AuditEntries { get; set; } = null!;
    public DbSet<AccessLog> AccessLogs { get; set; } = null!;
    public DbSet<DataChangeAudit> DataChangeAudits { get; set; } = null!;
    public DbSet<ComplianceReport> ComplianceReports { get; set; } = null!;
    public DbSet<AuditLogExport> AuditLogExports { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AuditEntry - immutable, append-only
        modelBuilder.Entity<AuditEntry>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => new { x.ResourceType, x.ResourceId });
            e.HasIndex(x => x.Timestamp).IsDescending();
            e.HasIndex(x => x.Action);
            e.HasIndex(x => x.Status);
            e.Property(x => x.IntegrityHash).IsRequired();
        });

        // AccessLog
        modelBuilder.Entity<AccessLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => new { x.ResourceType, x.ResourceId });
            e.HasIndex(x => x.AccessedAt).IsDescending();
        });

        // DataChangeAudit - append-only
        modelBuilder.Entity<DataChangeAudit>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => new { x.ResourceType, x.ResourceId });
            e.HasIndex(x => x.FieldName);
            e.HasIndex(x => x.ChangedAt).IsDescending();
        });

        // ComplianceReport
        modelBuilder.Entity<ComplianceReport>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.PeriodStart, x.PeriodEnd });
            e.HasIndex(x => x.Status);
        });

        // AuditLogExport
        modelBuilder.Entity<AuditLogExport>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ExportedAt).IsDescending();
            e.HasIndex(x => x.Status);
            e.Property(x => x.FileHash).IsRequired();
        });
    }
}
