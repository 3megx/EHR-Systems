using Mapster;
using EHRPlatform.Common.Mapping;
using EHRPlatform.Services.Patient.Features.Patients.Domain;
using EHRPlatform.Services.Patient.Features.Patients.Queries;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Patient.Mappings;

/// <summary>
/// Patient Mapper
/// Single Responsibility: Convert between Patient domain models and DTOs.
/// Handles all Patient-related mappings with optional post-processing.
/// </summary>
public class PatientMapper : MappingServiceBase<Domain.Patient, PatientResponseDto>
{
    public PatientMapper(ILogger<PatientMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single patient to response DTO.
    /// </summary>
    public PatientResponseDto MapToResponseDto(Domain.Patient patient)
    {
        return MapToDto(patient);
    }

    /// <summary>
    /// Map patient to detailed DTO with allergies and conditions.
    /// </summary>
    public PatientDetailDto MapToDetailDto(Domain.Patient patient)
    {
        Logger.LogDebug("Mapping patient {PatientId} to detailed DTO", patient.Id);

        var age = DateTime.UtcNow.Year - patient.DateOfBirth.Year;
        if (patient.DateOfBirth.Date > DateTime.UtcNow.AddYears(-age).Date)
            age--;

        return new PatientDetailDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            DateOfBirth = patient.DateOfBirth,
            Age = age,
            Gender = patient.Gender,
            MRN = patient.MRN,
            BloodType = patient.BloodType,
            EmergencyContact = patient.EmergencyContact,
            EmergencyPhone = patient.EmergencyPhone,
            Status = patient.Status,
            Allergies = patient.Allergies.Adapt<List<AllergyDetailDto>>(),
            Conditions = patient.Conditions.Adapt<List<ConditionDetailDto>>(),
            CreatedAt = patient.CreatedAt,
            LastModifiedAt = patient.LastModifiedAt
        };
    }

    /// <summary>
    /// Map collection of patients to paginated DTO.
    /// </summary>
    public PatientListDto MapToListDto(
        ICollection<Domain.Patient> patients,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} patients to paginated list DTO", patients.Count);

        return new PatientListDto
        {
            Items = patients.Adapt<List<PatientResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Map collection of patients to response DTO list.
    /// </summary>
    public List<PatientResponseDto> MapToResponseDtoList(ICollection<Domain.Patient> patients)
    {
        Logger.LogDebug("Mapping {Count} patients to response DTO list", patients.Count);
        return patients.Adapt<List<PatientResponseDto>>();
    }

    /// <summary>
    /// Map search results with pagination.
    /// </summary>
    public SearchResultDto<PatientResponseDto> MapToSearchResultDto(
        ICollection<Domain.Patient> patients,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} patients to search result DTO", patients.Count);

        return new SearchResultDto<PatientResponseDto>
        {
            Items = patients.Adapt<List<PatientResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
