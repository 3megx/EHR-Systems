namespace EHRPlatform.Services.Audit.Features.Audit.Dtos.Responses;

/// <summary>
/// Compliance report DTO.
/// Single Responsibility: Represent compliance audit summary for period.
/// </summary>
public class ComplianceReportDto
{
    public Guid Id { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalActions { get; set; }
    public int FailedActions { get; set; }
    public int DataAccess { get; set; }
    public int DataChanges { get; set; }
    public int UnauthorizedAttempts { get; set; }
    public List<string> PiiAccessed { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string? GeneratedBy { get; set; }
}
