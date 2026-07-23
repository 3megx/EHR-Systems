using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Audit.Domain.Entities;

namespace EHRPlatform.Services.Audit.Data.Configuration;

/// <summary>
/// Entity configuration for ComplianceReport (HIPAA compliance).
/// </summary>
public class ComplianceReportConfiguration : IEntityTypeConfiguration<ComplianceReport>
{
    public void Configure(EntityTypeBuilder<ComplianceReport> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => new { e.PeriodStart, e.PeriodEnd });
        entity.HasIndex(e => e.Status);
        entity.Property(e => e.ReportType).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Status).HasMaxLength(50);
    }
}
