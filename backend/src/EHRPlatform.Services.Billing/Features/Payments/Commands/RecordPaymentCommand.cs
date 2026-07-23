using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Billing.Features.Payments.Commands;

/// <summary>
/// Record payment command.
/// </summary>
public record RecordPaymentCommand : ICommand
{
    public Guid InvoiceId { get; init; }
    public decimal Amount { get; init; }
    public string Method { get; init; } = string.Empty; // Credit Card, Check, ACH, Insurance
    public string? Reference { get; init; }
}

public class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Payment amount must be greater than zero");
        RuleFor(x => x.Method).NotEmpty();
    }
}
