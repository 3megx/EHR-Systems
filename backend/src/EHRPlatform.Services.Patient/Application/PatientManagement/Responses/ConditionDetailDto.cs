namespace EHRPlatform.Services.Patient.Application.PatientManagement.Responses;

/// <summary>
/// Condition detail DTO.
/// Single Responsibility: Represent patient medical condition.
/// </summary>
public class ConditionDetailDto
{
    public Guid Id { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string ICD10Code { get; set; } = string.Empty;
    public DateTime? OnsetDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

