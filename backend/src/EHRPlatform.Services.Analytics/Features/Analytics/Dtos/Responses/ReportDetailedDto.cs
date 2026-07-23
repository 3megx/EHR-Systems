namespace EHRPlatform.Services.Analytics.Features.Analytics.Dtos.Responses;

/// <summary>
/// Report detailed DTO.
/// Single Responsibility: Represent complete report with execution history.
/// </summary>
public class ReportDetailedDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public List<string> Metrics { get; set; } = new();
    public string Schedule { get; set; } = string.Empty;
    public DateTime? LastGeneratedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public List<ReportExecutionDto> Executions { get; set; } = new();
}
