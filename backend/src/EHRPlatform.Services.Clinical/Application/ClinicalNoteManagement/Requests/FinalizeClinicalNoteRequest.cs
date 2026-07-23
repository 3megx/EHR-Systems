namespace EHRPlatform.Services.Clinical.Application.ClinicalNoteManagement.Requests;

/// <summary>
/// Finalize clinical note request.
/// </summary>
public class FinalizeClinicalNoteRequest
{
    public Guid ClinicalNoteId { get; set; }
}
