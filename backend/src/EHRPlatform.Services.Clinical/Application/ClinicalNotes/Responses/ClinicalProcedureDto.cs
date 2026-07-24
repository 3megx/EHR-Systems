namespace EHRPlatform.Services.Clinical.Application.ClinicalNotes.Responses;

/// <summary>
/// Clinical procedure nested DTO.
/// </summary>
public class ClinicalProcedureDto
{
    public Guid Id { get; set; }
    public string ProcedureName { get; set; } = string.Empty;
    public string ProcedureCode { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
}
