using FluentValidation;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Commands;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Validation;

/// <summary>
/// Validator for CreateClinicalNoteCommand.
/// </summary>
public class CreateClinicalNoteValidator : AbstractValidator<CreateClinicalNoteCommand>
{
    public CreateClinicalNoteValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required");

        RuleFor(x => x.ProviderId)
            .NotEmpty().WithMessage("ProviderId is required");

        RuleFor(x => x.EncounterDate)
            .NotEmpty().WithMessage("EncounterDate is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("EncounterDate cannot be in the future");

        RuleFor(x => x.EncounterType)
            .NotEmpty().WithMessage("EncounterType is required")
            .Must(x => new[] { "Office", "Telehealth", "Emergency", "Hospital" }.Contains(x))
            .WithMessage("EncounterType must be Office, Telehealth, Emergency, or Hospital");
    }
}
