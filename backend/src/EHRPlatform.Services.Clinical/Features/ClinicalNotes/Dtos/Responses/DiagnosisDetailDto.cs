namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Dtos.Responses;

/// <summary>
/// Diagnosis detail DTO.
/// Single Responsibility: Represent diagnosis history with all details.
/// </summary>
public class DiagnosisDetailDto
{
    public Guid PatientId { get; set; }
    public List<DiagnosisHistoryItemDto> Diagnoses { get; set; } = new();
}

public class DiagnosisHistoryItemDto
{
    public Guid Id { get; set; }
    public string DiagnosisCode { get; set; } = string.Empty;
    public string DiagnosisText { get; set; } = string.Empty;
    public string DiagnosisType { get; set; } = string.Empty;
    public DateTime RecordedDate { get; set; }
    public Guid ClinicalNoteId { get; set; }
}
