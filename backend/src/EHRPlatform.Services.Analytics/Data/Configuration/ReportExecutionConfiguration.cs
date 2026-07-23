using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EHRPlatform.Services.Analytics.Domain.Entities;

namespace EHRPlatform.Services.Analytics.Data.Configuration;

/// <summary>
/// Entity configuration for ReportExecution.
/// </summary>
public class ReportExecutionConfiguration : IEntityTypeConfiguration<ReportExecution>
{
    public void Configure(EntityTypeBuilder<ReportExecution> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasOne(e => e.Report).WithMany(r => r.Executions).HasForeignKey(e => e.ReportId);
        entity.HasIndex(e => e.ExecutedAt).IsDescending();
        entity.Property(e => e.Status).HasMaxLength(50);
    }
}
