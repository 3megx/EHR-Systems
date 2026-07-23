namespace EHRPlatform.Services.Clinical.Application.ClinicalNoteManagement.Requests;

/// <summary>
/// Add procedure request.
/// </summary>
public class AddProcedureRequest
{
    public Guid ClinicalNoteId { get; set; }
    public string ProcedureName { get; set; } = string.Empty;
    public string ProcedureCode { get; set; } = string.Empty; // CPT or SNOMED
    public string Result { get; set; } = string.Empty;
}
