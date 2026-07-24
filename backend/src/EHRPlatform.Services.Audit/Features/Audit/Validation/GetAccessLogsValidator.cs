using FluentValidation;
using EHRPlatform.Services.Audit.Features.Audit.Queries;

namespace EHRPlatform.Services.Audit.Features.Audit.Validation;

/// <summary>
/// Validator for GetAccessLogsQuery.
/// </summary>
public class GetAccessLogsValidator : AbstractValidator<GetAccessLogsQuery>
{
    public GetAccessLogsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("PageSize must not exceed 1000");

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("StartDate must be before EndDate");
    }
}
