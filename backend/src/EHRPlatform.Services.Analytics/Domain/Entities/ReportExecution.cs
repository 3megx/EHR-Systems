using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Analytics.Domain.Entities;

/// <summary>
/// Report execution record.
/// </summary>
public class ReportExecution : BaseEntity
{
    public Guid ReportId { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Running, Completed, Failed
    public string? OutputPath { get; set; }
    public long? FileSize { get; set; }
    public string? ErrorMessage { get; set; }
    public int RecordCount { get; set; }
    public Report Report { get; set; } = null!;
}
