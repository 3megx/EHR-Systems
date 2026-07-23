namespace EHRPlatform.Services.Patient.Application.PatientManagement.Responses;

/// <summary>
/// Generic search result wrapper with pagination metadata.
/// Single Responsibility: Encapsulate paginated search results with metadata.
/// </summary>
public class SearchResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (Total + PageSize - 1) / PageSize;
}

