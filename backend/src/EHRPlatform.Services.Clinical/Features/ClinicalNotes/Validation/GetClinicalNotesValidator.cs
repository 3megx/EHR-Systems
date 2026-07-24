using FluentValidation;
using EHRPlatform.Services.Clinical.Features.ClinicalNotes.Queries;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Validation;

/// <summary>
/// Validator for GetClinicalNotesQuery.
/// </summary>
public class GetClinicalNotesValidator : AbstractValidator<GetClinicalNotesQuery>
{
    public GetClinicalNotesValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("PageSize must not exceed 1000");

        RuleFor(x => x.Status)
            .Must(x => x == null || new[] { "Draft", "Finalized", "Locked" }.Contains(x))
            .WithMessage("Status must be Draft, Finalized, or Locked");
    }
}
