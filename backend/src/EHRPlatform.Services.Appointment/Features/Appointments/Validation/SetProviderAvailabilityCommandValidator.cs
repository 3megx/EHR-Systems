using FluentValidation;
using EHRPlatform.Services.Appointment.Features.Appointments.Commands;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Validation;

public class SetProviderAvailabilityCommandValidator : AbstractValidator<SetProviderAvailabilityCommand>
{
    public SetProviderAvailabilityCommandValidator()
    {
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.SlotStart).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.SlotEnd).GreaterThan(x => x.SlotStart);
    }
}
