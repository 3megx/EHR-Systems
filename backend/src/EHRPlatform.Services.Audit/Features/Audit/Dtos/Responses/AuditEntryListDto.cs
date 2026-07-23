namespace EHRPlatform.Services.Audit.Features.Audit.Dtos.Responses;

/// <summary>
/// Audit entry list DTO.
/// Single Responsibility: Represent paginated audit entries for a resource.
/// </summary>
public class AuditEntryListDto
{
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public List<AuditEntryResponseDto> Entries { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
