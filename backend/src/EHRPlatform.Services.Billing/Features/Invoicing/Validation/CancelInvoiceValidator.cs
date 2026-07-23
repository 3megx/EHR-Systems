using FluentValidation;
using EHRPlatform.Services.Billing.Features.Invoicing.Commands;

namespace EHRPlatform.Services.Billing.Features.Invoicing.Validation;

public class CancelInvoiceValidator : AbstractValidator<CancelInvoiceCommand>
{
    public CancelInvoiceValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
    }
}
