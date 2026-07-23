namespace EHRPlatform.Services.Clinical.Application.ClinicalNoteManagement.Requests;

/// <summary>
/// Add diagnosis request.
/// </summary>
public class AddDiagnosisRequest
{
    public Guid ClinicalNoteId { get; set; }
    public string DiagnosisCode { get; set; } = string.Empty; // ICD-10
    public string DiagnosisText { get; set; } = string.Empty;
    public string DiagnosisType { get; set; } = "Secondary"; // Principal or Secondary
}
