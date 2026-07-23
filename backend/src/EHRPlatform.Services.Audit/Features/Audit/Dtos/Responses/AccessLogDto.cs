namespace EHRPlatform.Services.Audit.Features.Audit.Dtos.Responses;

/// <summary>
/// Access log DTO.
/// Single Responsibility: Represent user access activity summary.
/// </summary>
public class AccessLogDto
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public List<ActivitySummaryDto> Activities { get; set; } = new();
    public int TotalActions { get; set; }
    public int FailedActions { get; set; }
    public DateTime? FirstActionAt { get; set; }
    public DateTime? LastActionAt { get; set; }
}

public class ActivitySummaryDto
{
    public string Action { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime LastOccurred { get; set; }
}
