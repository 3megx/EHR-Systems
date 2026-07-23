namespace EHRPlatform.Services.Clinical.Application.Clinical.Responses;

public class ClinicalNoteResponse
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public string? EncounterType { get; set; }
    public string? ChiefComplaint { get; set; }
    public string? Status { get; set; }
    public DateTime EncounterDate { get; set; }
}
