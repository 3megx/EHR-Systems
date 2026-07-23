using FluentValidation;
using EHRPlatform.Services.Patient.Features.Patients.Commands;

namespace EHRPlatform.Services.Patient.Features.Patients.Validation;

public class CreatePatientValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.MRN).NotEmpty();
    }
}

public class CreatePatientCommand
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MRN { get; set; }
}
