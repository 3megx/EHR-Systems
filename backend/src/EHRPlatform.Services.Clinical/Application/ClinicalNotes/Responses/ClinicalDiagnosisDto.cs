namespace EHRPlatform.Services.Clinical.Application.ClinicalNotes.Responses;

/// <summary>
/// Clinical diagnosis nested DTO.
/// </summary>
public class ClinicalDiagnosisDto
{
    public Guid Id { get; set; }
    public string DiagnosisCode { get; set; } = string.Empty;
    public string DiagnosisText { get; set; } = string.Empty;
    public string DiagnosisType { get; set; } = string.Empty;
}
