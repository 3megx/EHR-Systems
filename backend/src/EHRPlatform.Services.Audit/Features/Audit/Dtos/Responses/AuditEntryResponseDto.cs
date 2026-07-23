namespace EHRPlatform.Services.Audit.Features.Audit.Dtos.Responses;

/// <summary>
/// Audit entry response DTO.
/// Single Responsibility: Represent individual audit log entry.
/// </summary>
public class AuditEntryResponseDto
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
}
