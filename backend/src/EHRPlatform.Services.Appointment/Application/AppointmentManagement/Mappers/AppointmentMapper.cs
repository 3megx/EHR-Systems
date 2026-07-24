using Mapster;
using EHRPlatform.Common.Mapping;
using EHRPlatform.Services.Appointment.Application.AppointmentManagement.Responses;
using EHRPlatform.Services.Appointment.Features.Appointments.Domain;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Appointment.Application.AppointmentManagement.Mappers;

/// <summary>
/// Appointment mapper.
/// Single Responsibility: convert Appointment domain model to DTOs.
/// </summary>
public class AppointmentMapper : MappingServiceBase<Appointment, AppointmentResponseDto>
{
    public AppointmentMapper(ILogger<AppointmentMapper> logger) : base(logger) { }

    /// <summary>Map single appointment to response DTO.</summary>
    public AppointmentResponseDto MapToResponseDto(Appointment appointment)
        => MapToDto(appointment);

    /// <summary>Map collection of appointments to response DTO list.</summary>
    public List<AppointmentResponseDto> MapToResponseDtoList(ICollection<Appointment> appointments)
    {
        Logger.LogDebug("Mapping {Count} appointments to response DTO list", appointments.Count);
        return appointments.Adapt<List<AppointmentResponseDto>>();
    }

    /// <summary>Map appointments to paginated list DTO.</summary>
    public AppointmentListDto MapToListDto(IList<Appointment> appointments, int total, int pageNumber, int pageSize)
    {
        return new AppointmentListDto
        {
            Items = appointments.Adapt<List<AppointmentResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>Map appointments to provider calendar DTO.</summary>
    public ProviderAppointmentCalendarDto MapToProviderCalendarDto(Guid providerId, DateTime date, IList<Appointment> appointments)
    {
        return new ProviderAppointmentCalendarDto
        {
            ProviderId = providerId,
            Date = date,
            Slots = appointments.Select(a => new AppointmentSlotDto
            {
                AppointmentId = a.Id,
                PatientId = a.PatientId,
                Start = a.ScheduledStart,
                End = a.ScheduledEnd,
                AppointmentType = a.AppointmentType,
                Status = a.Status
            }).ToList()
        };
    }

    /// <summary>Map provider availability slots to list DTO.</summary>
    public ProviderAvailabilityListDto MapToAvailabilityListDto(Guid providerId, IList<ProviderAvailability> slots)
    {
        return new ProviderAvailabilityListDto
        {
            ProviderId = providerId,
            Slots = slots.Select(s => new ProviderAvailabilitySlotDto
            {
                Id = s.Id,
                SlotStart = s.SlotStart,
                SlotEnd = s.SlotEnd,
                IsRecurring = s.IsRecurring,
                RecurrencePattern = s.RecurrencePattern,
                MaxAppointmentsPerSlot = s.MaxAppointmentsPerSlot,
                CurrentBookings = s.CurrentBookings,
                HasAvailability = s.HasAvailability()
            }).ToList()
        };
    }
}
