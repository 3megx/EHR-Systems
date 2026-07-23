using FluentValidation;
using EHRPlatform.Services.Appointment.Application.AppointmentBooking.Handlers;

namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Validators;

/// <summary>
/// Validator for ScheduleAppointmentCommand.
/// </summary>
public class ScheduleAppointmentValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.ScheduledStart).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(480); // Max 8 hours
        RuleFor(x => x.AppointmentType).Must(t => new[] { "Office", "Telehealth", "Phone" }.Contains(t));
    }
}
