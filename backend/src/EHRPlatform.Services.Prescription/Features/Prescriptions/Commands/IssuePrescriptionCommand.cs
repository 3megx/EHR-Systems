using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Prescription.Features.Prescriptions.Dtos.Responses;
using FluentValidation;

namespace EHRPlatform.Services.Prescription.Features.Prescriptions.Commands;

/// <summary>
/// Issue prescription command.
/// </summary>
public record IssuePrescriptionCommand : ICommand<PrescriptionResponseDto>
{
    public Guid PatientId { get; init; }
    public Guid ProviderId { get; init; }
    public string MedicationName { get; init; } = string.Empty;
    public string Strength { get; init; } = string.Empty;
    public string FormType { get; init; } = string.Empty;
    public string Dosage { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public int RefillsAllowed { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Indications { get; init; }
    public string? SpecialInstructions { get; init; }
    public bool IsControlledSubstance { get; init; }
    public string? NDCCode { get; init; }
}

public class IssuePrescriptionCommandValidator : AbstractValidator<IssuePrescriptionCommand>
{
    public IssuePrescriptionCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.MedicationName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Strength).NotEmpty();
        RuleFor(x => x.Dosage).NotEmpty();
        RuleFor(x => x.Frequency).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.RefillsAllowed).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StartDate).LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).When(x => x.EndDate.HasValue);
    }
}

/// <summary>
/// Request refill command.
/// </summary>
public record RequestRefillCommand : ICommand
{
    public Guid PrescriptionId { get; init; }
    public string? PharmacyId { get; init; }
}

/// <summary>
/// Approve refill command.
/// </summary>
public record ApproveRefillCommand : ICommand
{
    public Guid PrescriptionId { get; init; }
    public Guid RefillId { get; init; }
}

/// <summary>
/// Suspend prescription command.
/// </summary>
public record SuspendPrescriptionCommand : ICommand
{
    public Guid PrescriptionId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Resume prescription command.
/// </summary>
public record ResumePrescriptionCommand : ICommand
{
    public Guid PrescriptionId { get; init; }
}

/// <summary>
/// Discontinue prescription command.
/// </summary>
public record DiscontinuePrescriptionCommand : ICommand
{
    public Guid PrescriptionId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
