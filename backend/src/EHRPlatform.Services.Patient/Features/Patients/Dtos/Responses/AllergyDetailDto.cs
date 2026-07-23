namespace EHRPlatform.Services.Patient.Features.Patients.Dtos.Responses;

/// <summary>
/// Allergy detail DTO.
/// Single Responsibility: Represent patient allergy information.
/// </summary>
public class AllergyDetailDto
{
    public Guid Id { get; set; }
    public string Allergen { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
