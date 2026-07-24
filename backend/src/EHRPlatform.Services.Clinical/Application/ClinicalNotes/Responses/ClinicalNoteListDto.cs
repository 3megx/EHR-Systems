namespace EHRPlatform.Services.Clinical.Application.ClinicalNotes.Responses;

/// <summary>
/// Paginated list of clinical notes response DTO.
/// </summary>
public class ClinicalNoteListDto
{
    public List<ClinicalNoteResponse> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (Total + PageSize - 1) / PageSize;
}
