namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Dtos.Responses;

/// <summary>
/// Clinical note list DTO.
/// Single Responsibility: Represent clinical notes in list/timeline responses.
/// </summary>
public class ClinicalNoteListDto
{
    public Guid PatientId { get; set; }
    public List<ClinicalNoteTimelineItemDto> Notes { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class ClinicalNoteTimelineItemDto
{
    public Guid Id { get; set; }
    public DateTime EncounterDate { get; set; }
    public string EncounterType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid ProviderId { get; set; }
    public List<DiagnosisDto> Diagnoses { get; set; } = new();
    public VitalSignsDto? LatestVitals { get; set; }
}
