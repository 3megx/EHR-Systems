using Mapster;
using EHRPlatform.Common.Mapping;
using Microsoft.Extensions.Logging;
using EHRPlatform.Services.Appointment.Application.AppointmentManagement.Responses;

namespace EHRPlatform.Services.Appointment.Application.AppointmentManagement.Mappers;

/// <summary>
/// Appointment Mapper.
/// Single Responsibility: Convert between Appointment domain model and DTOs.
/// </summary>
public class AppointmentMapper : MappingServiceBase<Entities.Appointment, AppointmentResponseDto>
{
    public AppointmentMapper(ILogger<AppointmentMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single appointment to response DTO.
    /// </summary>
    public AppointmentResponseDto MapToResponseDto(Entities.Appointment appointment)
    {
        return MapToDto(appointment);
    }

    /// <summary>
    /// Map collection of appointments to response DTO list.
    /// </summary>
    public List<AppointmentResponseDto> MapToResponseDtoList(ICollection<Entities.Appointment> appointments)
    {
        Logger.LogDebug("Mapping {Count} appointments to response DTO list", appointments.Count);
        return appointments.Adapt<List<AppointmentResponseDto>>();
    }
}
