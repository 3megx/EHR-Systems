namespace EHRPlatform.Services.Analytics.Application.Analytics.Responses;

/// <summary>
/// Paginated list of dashboards response DTO.
/// </summary>
public class DashboardListDto
{
    public List<DashboardResponse> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (Total + PageSize - 1) / PageSize;
}
