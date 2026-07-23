using Mapster;
using EHRPlatform.Services.Patient.Features.Patients.Domain;
using EHRPlatform.Services.Patient.Features.Patients.Queries;

namespace EHRPlatform.Services.Patient.Mappings;

/// <summary>
/// Mapster registration profile for Patient entity mappings.
/// Handles conversion between domain models and DTOs.
/// Single Responsibility: Configure all Patient-related type mappings.
/// </summary>
public class PatientMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Patient → PatientResponseDto
        config.NewConfig<Domain.Patient, PatientResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.MRN, src => src.MRN)
            .Map(dest => dest.BloodType, src => src.BloodType)
            .Map(dest => dest.EmergencyContact, src => src.EmergencyContact)
            .Map(dest => dest.EmergencyPhone, src => src.EmergencyPhone)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.LastModifiedAt, src => src.LastModifiedAt);

        // PatientAllergy → AllergyDetailDto
        config.NewConfig<PatientAllergy, AllergyDetailDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Allergen, src => src.Allergen)
            .Map(dest => dest.Severity, src => src.Severity)
            .Map(dest => dest.Notes, src => src.Notes)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // PatientCondition → ConditionDetailDto
        config.NewConfig<PatientCondition, ConditionDetailDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Condition, src => src.Condition)
            .Map(dest => dest.ICD10Code, src => src.ICD10Code)
            .Map(dest => dest.OnsetDate, src => src.OnsetDate)
            .Map(dest => dest.ResolvedDate, src => src.ResolvedDate)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // PatientResponseDto → Patient (for updates)
        config.NewConfig<PatientResponseDto, Domain.Patient>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.MRN, src => src.MRN)
            .Map(dest => dest.BloodType, src => src.BloodType)
            .Map(dest => dest.EmergencyContact, src => src.EmergencyContact)
            .Map(dest => dest.EmergencyPhone, src => src.EmergencyPhone)
            .Map(dest => dest.Status, src => src.Status);
    }
}

/// <summary>
/// Patient response DTO (basic info).
/// </summary>
public class PatientResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MRN { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

/// <summary>
/// Patient list DTO with pagination.
/// </summary>
public class PatientListDto
{
    public List<PatientResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
