using FluentValidation;
using EHRPlatform.Services.Billing.Features.Reports.Queries;

namespace EHRPlatform.Services.Billing.Features.Reports.Validation;

public class GetPatientInvoicesValidator : AbstractValidator<GetPatientInvoicesQuery>
{
    public GetPatientInvoicesValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(1000);
    }
}

public class GetPatientOutstandingBalanceValidator : AbstractValidator<GetPatientOutstandingBalanceQuery>
{
    public GetPatientOutstandingBalanceValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
    }
}

public class GetBillingReportValidator : AbstractValidator<GetBillingReportQuery>
{
    public GetBillingReportValidator()
    {
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
    }
}
