namespace EHRPlatform.Services.Audit.Application.Audit.Responses;

/// <summary>
/// Response DTO for AccessLog.
/// </summary>
public class AccessLogResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public DateTime AccessedAt { get; set; }
}
