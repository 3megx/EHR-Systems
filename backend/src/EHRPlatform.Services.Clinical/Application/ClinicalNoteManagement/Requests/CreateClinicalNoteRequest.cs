namespace EHRPlatform.Services.Clinical.Application.ClinicalNoteManagement.Requests;

/// <summary>
/// Create clinical note request.
/// </summary>
public class CreateClinicalNoteRequest
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime EncounterDate { get; set; }
    public string EncounterType { get; set; } = string.Empty; // Office, Telehealth, Emergency, Hospital
}
