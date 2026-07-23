using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Appointment.Features.Appointments.Dtos.Responses;
using FluentValidation;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Commands;

/// <summary>
/// Schedule appointment command.
/// </summary>
public record ScheduleAppointmentCommand : ICommand<AppointmentResponseDto>
{
    public Guid PatientId { get; init; }
    public Guid ProviderId { get; init; }
    public DateTime ScheduledStart { get; init; }
    public int DurationMinutes { get; init; }
    public string AppointmentType { get; init; } = string.Empty; // Office, Telehealth, Phone
    public string? ReasonForVisit { get; init; }
    public string? Notes { get; init; }
}

public class ScheduleAppointmentCommandValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.ScheduledStart).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(480); // Max 8 hours
        RuleFor(x => x.AppointmentType).Must(t => new[] { "Office", "Telehealth", "Phone" }.Contains(t));
    }
}

/// <summary>
/// Confirm appointment command.
/// </summary>
public record ConfirmAppointmentCommand : ICommand
{
    public Guid AppointmentId { get; init; }
}

/// <summary>
/// Cancel appointment command.
/// </summary>
public record CancelAppointmentCommand : ICommand
{
    public Guid AppointmentId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Check-in appointment command.
/// </summary>
public record CheckInAppointmentCommand : ICommand
{
    public Guid AppointmentId { get; init; }
}

/// <summary>
/// Complete appointment command.
/// </summary>
public record CompleteAppointmentCommand : ICommand
{
    public Guid AppointmentId { get; init; }
}

/// <summary>
/// Set provider availability command.
/// </summary>
public record SetProviderAvailabilityCommand : ICommand<ProviderAvailabilityDto>
{
    public Guid ProviderId { get; init; }
    public DateTime SlotStart { get; init; }
    public DateTime SlotEnd { get; init; }
    public bool IsRecurring { get; init; }
    public string? RecurrencePattern { get; init; } // Daily, Weekly, Monthly
    public int? MaxAppointmentsPerSlot { get; init; }
}

public class SetProviderAvailabilityCommandValidator : AbstractValidator<SetProviderAvailabilityCommand>
{
    public SetProviderAvailabilityCommandValidator()
    {
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.SlotStart).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.SlotEnd).GreaterThan(x => x.SlotStart);
    }
}
