using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Clinical.Application.ClinicalNoteManagement.Responses;
using FluentValidation;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Commands;

/// <summary>
/// Create clinical note command.
/// Initializes SOAP note in draft status.
/// </summary>
public record CreateClinicalNoteCommand : ICommand<ClinicalNoteResponseDto>
{
    public Guid PatientId { get; init; }
    public Guid ProviderId { get; init; }
    public DateTime EncounterDate { get; init; }
    public string EncounterType { get; init; } = string.Empty; // Office, Telehealth, Emergency, Hospital
}

public class CreateClinicalNoteCommandValidator : AbstractValidator<CreateClinicalNoteCommand>
{
    public CreateClinicalNoteCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.EncounterDate).LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.EncounterType).Must(t => new[] { "Office", "Telehealth", "Emergency", "Hospital" }.Contains(t));
    }
}
