using FluentValidation;
using EHRPlatform.Services.Billing.Features.Invoicing.Commands;

namespace EHRPlatform.Services.Billing.Features.Invoicing.Validation;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ServiceDate).LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.LineItems).NotEmpty().WithMessage("At least one line item required");
        RuleForEach(x => x.LineItems).SetValidator(new LineItemValidator());
    }
}

public class LineItemValidator : AbstractValidator<LineItemRequest>
{
    public LineItemValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CPTCode).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThan(0);
    }
}
