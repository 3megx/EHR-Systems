using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Patient.Features.Patients.Commands;

/// <summary>
/// Create patient command.
/// </summary>
public record CreatePatientCommand : ICommand<PatientResponseDto>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string BloodType { get; init; } = string.Empty;
    public string? EmergencyContact { get; init; }
    public string? EmergencyPhone { get; init; }
}

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.PhoneNumber).Matches(@"^\+?[0-9]{10,}$");
        RuleFor(x => x.DateOfBirth).LessThan(DateTime.Now);
        RuleFor(x => x.Gender).Must(g => new[] { "M", "F", "Other" }.Contains(g));
    }
}

/// <summary>
/// Update patient command.
/// </summary>
public record UpdatePatientCommand : ICommand<PatientResponseDto>
{
    public Guid PatientId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string BloodType { get; init; } = string.Empty;
    public string? EmergencyContact { get; init; }
    public string? EmergencyPhone { get; init; }
}

/// <summary>
/// Add allergy command.
/// </summary>
public record AddAllergyCommand : ICommand
{
    public Guid PatientId { get; init; }
    public string Allergen { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty; // Mild, Moderate, Severe
    public string Notes { get; init; } = string.Empty;
}

public class AddAllergyCommandValidator : AbstractValidator<AddAllergyCommand>
{
    public AddAllergyCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Allergen).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Severity).Must(s => new[] { "Mild", "Moderate", "Severe" }.Contains(s));
    }
}

/// <summary>
/// Add condition command.
/// </summary>
public record AddConditionCommand : ICommand
{
    public Guid PatientId { get; init; }
    public string Condition { get; init; } = string.Empty;
    public string ICD10Code { get; init; } = string.Empty;
    public DateTime? OnsetDate { get; init; }
}

public class AddConditionCommandValidator : AbstractValidator<AddConditionCommand>
{
    public AddConditionCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Condition).NotEmpty();
        RuleFor(x => x.ICD10Code).Matches(@"^[A-Z][0-9]{2}(\.[0-9]{1,2})?$");
    }
}

/// <summary>
/// Patient response DTO.
/// </summary>
public class PatientResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MRN { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<AllergyDto> Allergies { get; set; } = new();
    public List<ConditionDto> Conditions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class AllergyDto
{
    public Guid Id { get; set; }
    public string Allergen { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class ConditionDto
{
    public Guid Id { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string ICD10Code { get; set; } = string.Empty;
    public DateTime? OnsetDate { get; set; }
}
