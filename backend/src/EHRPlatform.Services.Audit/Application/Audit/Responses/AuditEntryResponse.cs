namespace EHRPlatform.Services.Audit.Application.Audit.Responses;

/// <summary>
/// Response DTO for AuditEntry.
/// </summary>
public class AuditEntryResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? Action { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public string? Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}
