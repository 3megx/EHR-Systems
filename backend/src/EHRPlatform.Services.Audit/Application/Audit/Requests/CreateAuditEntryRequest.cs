namespace EHRPlatform.Services.Audit.Application.Audit.Requests;

/// <summary>
/// Request DTO for creating an audit entry.
/// </summary>
public class CreateAuditEntryRequest
{
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? Action { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public string? Details { get; set; }
}
