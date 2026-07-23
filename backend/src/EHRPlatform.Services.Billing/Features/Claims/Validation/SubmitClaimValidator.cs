using FluentValidation;
using EHRPlatform.Services.Billing.Features.Claims.Commands;

namespace EHRPlatform.Services.Billing.Features.Claims.Validation;

public class SubmitClaimValidator : AbstractValidator<SubmitToInsuranceCommand>
{
    public SubmitClaimValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.InsuranceProvider).NotEmpty();
        RuleFor(x => x.PolicyNumber).NotEmpty();
    }
}
