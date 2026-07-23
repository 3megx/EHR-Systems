using FluentValidation;
using EHRPlatform.Services.Billing.Features.Invoicing.Queries;

namespace EHRPlatform.Services.Billing.Features.Invoicing.Validation;

public class GetInvoiceValidator : AbstractValidator<GetInvoiceQuery>
{
    public GetInvoiceValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
    }
}
