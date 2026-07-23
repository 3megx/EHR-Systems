namespace EHRPlatform.Services.Clinical.Application.ClinicalNoteManagement.Requests;

/// <summary>
/// Update SOAP request.
/// </summary>
public class UpdateSOAPRequest
{
    public Guid ClinicalNoteId { get; set; }
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }
}
