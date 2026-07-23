namespace EHRPlatform.Services.Clinical.Application.Clinical.Requests;

public class CreateClinicalNoteRequest
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public string? EncounterType { get; set; }
    public string? ChiefComplaint { get; set; }
}
