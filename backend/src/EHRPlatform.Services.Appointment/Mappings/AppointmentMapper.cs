using Mapster;
using EHRPlatform.Common.Mapping;
using EHRPlatform.Services.Appointment.Features.Appointments.Domain;
using EHRPlatform.Services.Appointment.Features.Appointments.Dtos.Responses;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Appointment.Mappings;

/// <summary>
/// Appointment Mapper
/// Single Responsibility: Convert between Appointment domain models and DTOs.
/// Handles all Appointment-related mappings with optional post-processing.
/// </summary>
public class AppointmentMapper : MappingServiceBase<Appointment, AppointmentResponseDto>
{
    public AppointmentMapper(ILogger<AppointmentMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single appointment to response DTO.
    /// </summary>
    public AppointmentResponseDto MapToResponseDto(Appointment appointment)
    {
        return MapToDto(appointment);
    }

    /// <summary>
    /// Map collection of appointments to paginated DTO.
    /// </summary>
    public AppointmentListDto MapToListDto(
        ICollection<Appointment> appointments,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} appointments to paginated list DTO", appointments.Count);

        return new AppointmentListDto
        {
            Items = appointments.Adapt<List<AppointmentResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Map collection of appointments to response DTO list.
    /// </summary>
    public List<AppointmentResponseDto> MapToResponseDtoList(ICollection<Appointment> appointments)
    {
        Logger.LogDebug("Mapping {Count} appointments to response DTO list", appointments.Count);
        return appointments.Adapt<List<AppointmentResponseDto>>();
    }

    /// <summary>
    /// Map appointments to provider calendar view with slots.
    /// </summary>
    public ProviderAppointmentCalendarDto MapToProviderCalendarDto(
        Guid providerId,
        DateTime date,
        ICollection<Appointment> appointments)
    {
        Logger.LogDebug("Mapping calendar for provider {ProviderId} on {Date:yyyy-MM-dd}", providerId, date);

        var slots = appointments
            .Select(a => new AppointmentSlotDto
            {
                Start = a.ScheduledStart,
                End = a.ScheduledEnd,
                Status = a.Status == "Cancelled" ? "Available" 
                    : (a.Status == "Scheduled" || a.Status == "Confirmed" ? "Booked" : "Blocked"),
                AppointmentId = a.Id,
                PatientId = a.PatientId
            })
            .OrderBy(s => s.Start)
            .ToList();

        return new ProviderAppointmentCalendarDto
        {
            ProviderId = providerId,
            Date = date,
            Slots = slots
        };
    }

    /// <summary>
    /// Map provider availability slots.
    /// </summary>
    public ProviderAvailabilityListDto MapToAvailabilityListDto(
        Guid providerId,
        ICollection<ProviderAvailability> availabilities)
    {
        Logger.LogDebug("Mapping {Count} availability slots for provider {ProviderId}", 
            availabilities.Count, providerId);

        return new ProviderAvailabilityListDto
        {
            ProviderId = providerId,
            Slots = availabilities.Adapt<List<ProviderAvailabilitySlotDto>>()
                .OrderBy(s => s.SlotStart)
                .ToList()
        };
    }

    /// <summary>
    /// Map appointment to command DTO (for updates).
    /// </summary>
    public AppointmentCommandDto MapToCommandDto(Appointment appointment)
    {
        Logger.LogDebug("Mapping appointment {AppointmentId} to command DTO", appointment.Id);

        return new AppointmentCommandDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            ProviderId = appointment.ProviderId,
            ScheduledStart = appointment.ScheduledStart,
            ScheduledEnd = appointment.ScheduledEnd,
            AppointmentType = appointment.AppointmentType,
            Status = appointment.Status,
            ReasonForVisit = appointment.ReasonForVisit,
            Notes = appointment.Notes,
            DurationMinutes = appointment.DurationMinutes
        };
    }

    /// <summary>
    /// Map appointment with enriched reminder information.
    /// </summary>
    public AppointmentDetailedResponseDto MapToDetailedResponseDto(
        Appointment appointment)
    {
        Logger.LogDebug("Mapping appointment {AppointmentId} to detailed response", appointment.Id);

        var baseDto = appointment.Adapt<AppointmentResponseDto>();

        return new AppointmentDetailedResponseDto
        {
            Id = baseDto.Id,
            PatientId = baseDto.PatientId,
            ProviderId = baseDto.ProviderId,
            ScheduledStart = baseDto.ScheduledStart,
            ScheduledEnd = baseDto.ScheduledEnd,
            AppointmentType = baseDto.AppointmentType,
            Status = baseDto.Status,
            ReasonForVisit = baseDto.ReasonForVisit,
            Notes = baseDto.Notes,
            DurationMinutes = baseDto.DurationMinutes,
            ReminderSent = baseDto.ReminderSent,
            ConfirmedAt = baseDto.ConfirmedAt,
            CancelledAt = baseDto.CancelledAt,
            CancellationReason = baseDto.CancellationReason,
            Reminders = appointment.Reminders.Adapt<List<AppointmentReminderDto>>(),
            IsAvailable = appointment.IsAvailable,
            TimeUntilAppointment = appointment.ScheduledStart > DateTime.UtcNow 
                ? (appointment.ScheduledStart - DateTime.UtcNow).TotalMinutes 
                : 0
        };
    }
}
