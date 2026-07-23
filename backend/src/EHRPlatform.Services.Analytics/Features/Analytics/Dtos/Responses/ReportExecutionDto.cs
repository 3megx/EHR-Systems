namespace EHRPlatform.Services.Analytics.Features.Analytics.Dtos.Responses;

/// <summary>
/// Report response DTO.
/// Single Responsibility: Represent report metadata and summary.
/// </summary>
public class ReportResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public List<string> Metrics { get; set; } = new();
    public string Schedule { get; set; } = string.Empty;
    public DateTime? LastGeneratedAt { get; set; }
    public List<ReportExecutionDto> Executions { get; set; } = new();
}

/// <summary>
/// Report execution DTO.
/// Single Responsibility: Represent report execution instance.
/// </summary>
public class ReportExecutionDto
{
    public Guid Id { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? OutputPath { get; set; }
    public int RecordCount { get; set; }
}
