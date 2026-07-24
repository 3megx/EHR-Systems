namespace EHRPlatform.Services.Clinical.Application.ClinicalNotes.Responses;

/// <summary>
/// Clinical note response DTO with nested vitals, diagnoses, procedures.
/// </summary>
public class ClinicalNoteResponse
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

    // Nested collections
    public List<VitalSignsDto> VitalSigns { get; set; } = new();
    public List<ClinicalDiagnosisDto> Diagnoses { get; set; } = new();
    public List<ClinicalProcedureDto> Procedures { get; set; } = new();

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
