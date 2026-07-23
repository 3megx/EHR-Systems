using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Billing.Features.Claims.Commands;

/// <summary>
/// Submit to insurance command.
/// </summary>
public record SubmitToInsuranceCommand : ICommand
{
    public Guid InvoiceId { get; init; }
    public string InsuranceProvider { get; init; } = string.Empty;
    public string PolicyNumber { get; init; } = string.Empty;
}

public class SubmitToInsuranceCommandValidator : AbstractValidator<SubmitToInsuranceCommand>
{
    public SubmitToInsuranceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.InsuranceProvider).NotEmpty();
        RuleFor(x => x.PolicyNumber).NotEmpty();
    }
}

/// <summary>
/// Cancel invoice command.
/// </summary>
public record CancelInvoiceCommand : ICommand
{
    public Guid InvoiceId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public class CancelInvoiceCommandValidator : AbstractValidator<CancelInvoiceCommand>
{
    public CancelInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
    }
}
