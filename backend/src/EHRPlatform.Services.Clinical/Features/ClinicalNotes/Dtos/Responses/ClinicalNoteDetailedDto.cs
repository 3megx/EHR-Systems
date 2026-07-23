namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Dtos.Responses;

/// <summary>
/// Detailed clinical note DTO.
/// Single Responsibility: Represent complete clinical note with all details.
/// </summary>
public class ClinicalNoteDetailedDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime EncounterDate { get; set; }
    public string EncounterType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Subjective { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string Assessment { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public List<DiagnosisDto> Diagnoses { get; set; } = new();
    public List<ProcedureDto> Procedures { get; set; } = new();
    public VitalSignsDto? VitalSigns { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
