namespace EHRPlatform.Services.Audit.Application.Audit.Responses;

/// <summary>
/// Paginated list of audit entries response DTO.
/// </summary>
public class AuditListDto
{
    public List<AuditEntryResponse> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (Total + PageSize - 1) / PageSize;
}
