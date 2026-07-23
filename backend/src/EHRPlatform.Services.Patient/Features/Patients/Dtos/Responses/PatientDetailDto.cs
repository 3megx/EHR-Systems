namespace EHRPlatform.Services.Patient.Features.Patients.Dtos.Responses;

/// <summary>
/// Patient detail DTO with relationships.
/// Includes allergies, conditions, and calculated fields.
/// Single Responsibility: Represent enriched patient data for detailed views.
/// </summary>
public class PatientDetailDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MRN { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<AllergyDetailDto> Allergies { get; set; } = new();
    public List<ConditionDetailDto> Conditions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
