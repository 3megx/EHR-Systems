namespace EHRPlatform.Services.Audit.Application.Audit.Requests;

/// <summary>
/// Request DTO for getting audit entries.
/// </summary>
public class GetAuditEntriesRequest
{
    public Guid? UserId { get; set; }
    public string? ResourceType { get; set; }
    public string? Action { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
