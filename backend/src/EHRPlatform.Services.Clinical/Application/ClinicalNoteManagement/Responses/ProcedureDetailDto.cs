namespace EHRPlatform.Services.Clinical.Application.ClinicalNoteManagement.Responses;

/// <summary>
/// Procedure detail DTO.
/// Single Responsibility: Represent procedures performed during clinical encounters.
/// </summary>
public class ProcedureDetailDto
{
    public Guid Id { get; set; }
    public string ProcedureCode { get; set; } = string.Empty;
    public string ProcedureName { get; set; } = string.Empty;
    public DateTime PerformedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid ClinicalNoteId { get; set; }
}
