using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Analytics.Domain.Entities;

/// <summary>
/// Report template for scheduled generation.
/// </summary>
public class Report : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty; // Clinical, Financial, Operational
    public List<string> Metrics { get; set; } = new();
    public string Schedule { get; set; } = string.Empty; // Daily, Weekly, Monthly, OnDemand
    public DateTime? LastGeneratedAt { get; set; }
    public string? LastGeneratedPath { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ReportExecution> Executions { get; } = new List<ReportExecution>();
}
