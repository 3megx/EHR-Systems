using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Audit.Domain.Entities;

namespace EHRPlatform.Services.Audit.Data.Configuration;

/// <summary>
/// Entity configuration for ComplianceReport.
/// </summary>
public class ComplianceReportConfiguration : IEntityTypeConfiguration<ComplianceReport>
{
    public void Configure(EntityTypeBuilder<ComplianceReport> entity)
    {
        entity.HasKey(x => x.Id);
        entity.HasIndex(x => new { x.PeriodStart, x.PeriodEnd });
        entity.HasIndex(x => x.Status);
    }
}
