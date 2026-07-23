using FluentValidation;
using EHRPlatform.Services.Appointment.Features.Appointments.Commands;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Validation;

public class ScheduleAppointmentValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.ScheduledStart).GreaterThan(DateTime.UtcNow);
    }
}
