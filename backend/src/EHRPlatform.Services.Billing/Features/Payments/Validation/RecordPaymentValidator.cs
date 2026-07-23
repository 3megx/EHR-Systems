using FluentValidation;
using EHRPlatform.Services.Billing.Features.Payments.Commands;

namespace EHRPlatform.Services.Billing.Features.Payments.Validation;

public class RecordPaymentValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).NotEmpty();
    }
}
