using Mapster;
using EHRPlatform.Services.Appointment.Features.Appointments.Domain;
using EHRPlatform.Services.Appointment.Features.Appointments.Queries;

namespace EHRPlatform.Services.Appointment.Mappings;

/// <summary>
/// Mapster registration profile for Appointment entity mappings.
/// Handles conversion between domain models and DTOs.
/// Single Responsibility: Configure all Appointment-related type mappings.
/// </summary>
public class AppointmentMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Appointment → AppointmentResponseDto
        config.NewConfig<Appointment, AppointmentResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PatientId, src => src.PatientId)
            .Map(dest => dest.ProviderId, src => src.ProviderId)
            .Map(dest => dest.ScheduledStart, src => src.ScheduledStart)
            .Map(dest => dest.ScheduledEnd, src => src.ScheduledEnd)
            .Map(dest => dest.AppointmentType, src => src.AppointmentType)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.ReasonForVisit, src => src.ReasonForVisit)
            .Map(dest => dest.Notes, src => src.Notes)
            .Map(dest => dest.DurationMinutes, src => src.DurationMinutes)
            .Map(dest => dest.ReminderSent, src => src.ReminderSent)
            .Map(dest => dest.ConfirmedAt, src => src.ConfirmedAt)
            .Map(dest => dest.CancelledAt, src => src.CancelledAt)
            .Map(dest => dest.CancellationReason, src => src.CancellationReason);

        // AppointmentReminder → AppointmentReminderDto
        config.NewConfig<AppointmentReminder, AppointmentReminderDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ReminderTime, src => src.ReminderTime)
            .Map(dest => dest.Method, src => src.Method)
            .Map(dest => dest.IsSent, src => src.IsSent);

        // ProviderAvailability → ProviderAvailabilitySlotDto
        config.NewConfig<ProviderAvailability, ProviderAvailabilitySlotDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.SlotStart, src => src.SlotStart)
            .Map(dest => dest.SlotEnd, src => src.SlotEnd)
            .Map(dest => dest.IsRecurring, src => src.IsRecurring)
            .Map(dest => dest.RecurrencePattern, src => src.RecurrencePattern)
            .Map(dest => dest.MaxAppointmentsPerSlot, src => src.MaxAppointmentsPerSlot)
            .Map(dest => dest.CurrentBookings, src => src.CurrentBookings)
            .Map(dest => dest.HasAvailability, src => src.HasAvailability());

        // AppointmentResponseDto → Appointment (for updates)
        config.NewConfig<AppointmentResponseDto, Appointment>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PatientId, src => src.PatientId)
            .Map(dest => dest.ProviderId, src => src.ProviderId)
            .Map(dest => dest.ScheduledStart, src => src.ScheduledStart)
            .Map(dest => dest.ScheduledEnd, src => src.ScheduledEnd)
            .Map(dest => dest.AppointmentType, src => src.AppointmentType)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.ReasonForVisit, src => src.ReasonForVisit)
            .Map(dest => dest.Notes, src => src.Notes)
            .Map(dest => dest.DurationMinutes, src => src.DurationMinutes)
            .Map(dest => dest.ReminderSent, src => src.ReminderSent)
            .Map(dest => dest.ConfirmedAt, src => src.ConfirmedAt)
            .Map(dest => dest.CancelledAt, src => src.CancelledAt)
            .Map(dest => dest.CancellationReason, src => src.CancellationReason);
    }
}

/// <summary>
/// Appointment reminder DTO.
/// </summary>
public class AppointmentReminderDto
{
    public Guid Id { get; set; }
    public DateTime ReminderTime { get; set; }
    public string Method { get; set; } = string.Empty;
    public bool IsSent { get; set; }
}
