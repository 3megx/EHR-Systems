using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Billing.Application.Invoicing.Responses;
using EHRPlatform.Services.Billing.Application.Reports.Responses;

namespace EHRPlatform.Services.Billing.Features.Reports.Queries;

/// <summary>
/// Get billing report query.
/// </summary>
public record GetBillingReportQuery : ICachedQuery<BillingReportDto>
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    public string CacheKey => $"report_billing_{StartDate:yyyyMMdd}_{EndDate:yyyyMMdd}";
    public int CacheDurationSeconds => 3600;
}
