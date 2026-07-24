namespace EHRPlatform.Services.Audit.Application.Audit.Responses;

/// <summary>
/// Paginated list of access logs response DTO.
/// </summary>
public class AccessLogListDto
{
    public List<AccessLogResponse> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (Total + PageSize - 1) / PageSize;
}
