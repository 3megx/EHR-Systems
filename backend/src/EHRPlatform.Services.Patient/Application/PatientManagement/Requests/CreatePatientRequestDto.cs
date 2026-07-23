namespace EHRPlatform.Services.Patient.Application.PatientManagement.Requests;

/// <summary>
/// Create patient request DTO.
/// Contains information needed to create a new patient profile.
/// </summary>
public class CreatePatientRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
}

/// <summary>
/// Update patient request DTO.
/// Contains information needed to update an existing patient profile.
/// </summary>
public class UpdatePatientRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
}

/// <summary>
/// Add allergy request DTO.
/// </summary>
public class AddAllergyRequestDto
{
    public string Allergen { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Add condition request DTO.
/// </summary>
public class AddConditionRequestDto
{
    public string Condition { get; set; } = string.Empty;
    public string ICD10Code { get; set; } = string.Empty;
    public DateTime? OnsetDate { get; set; }
}
