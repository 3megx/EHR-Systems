namespace EHRPlatform.Services.Audit.Features.Audit.Dtos.Responses;

/// <summary>
/// Audit trail response DTO.
/// Single Responsibility: Represent complete audit trail for a resource.
/// </summary>
public class AuditTrailResponseDto
{
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public List<AuditEntryResponseDto> Entries { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
