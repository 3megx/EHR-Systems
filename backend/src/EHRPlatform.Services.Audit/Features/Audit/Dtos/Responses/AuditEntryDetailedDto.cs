namespace EHRPlatform.Services.Audit.Features.Audit.Dtos.Responses;

/// <summary>
/// Detailed audit entry DTO.
/// Single Responsibility: Represent complete audit entry with all context.
/// </summary>
public class AuditEntryDetailedDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? PiiIndicators { get; set; }
    public int AccessLevel { get; set; }
    public string? ChangeDetails { get; set; }
    public string? FailureReason { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public Dictionary<string, object>? ChangedFields { get; set; }
}
