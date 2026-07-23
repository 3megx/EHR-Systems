using Mapster;
using EHRPlatform.Common.Mapping;
using Microsoft.Extensions.Logging;
using EHRPlatform.Services.Patient.Application.PatientManagement.Responses;

namespace EHRPlatform.Services.Patient.Application.PatientManagement.Mappers;

/// <summary>
/// Patient Mapper.
/// Single Responsibility: Convert between Patient domain model and DTOs.
/// </summary>
public class PatientMapper : MappingServiceBase<Entities.Patient, PatientResponseDto>
{
    public PatientMapper(ILogger<PatientMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single patient to response DTO.
    /// </summary>
    public PatientResponseDto MapToResponseDto(Entities.Patient patient)
    {
        return MapToDto(patient);
    }

    /// <summary>
    /// Map collection of patients to response DTO list.
    /// </summary>
    public List<PatientResponseDto> MapToResponseDtoList(ICollection<Entities.Patient> patients)
    {
        Logger.LogDebug("Mapping {Count} patients to response DTO list", patients.Count);
        return patients.Adapt<List<PatientResponseDto>>();
    }

    /// <summary>
    /// Map patients to paginated list DTO.
    /// </summary>
    public PatientListDto MapToListDto(
        ICollection<Entities.Patient> patients,
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
}
