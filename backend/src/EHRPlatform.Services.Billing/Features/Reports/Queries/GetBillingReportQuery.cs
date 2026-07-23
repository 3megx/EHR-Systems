using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Billing.Application.Reports.Responses;

namespace EHRPlatform.Services.Billing.Features.Reports.Queries;

/// <summary>
/// Get patient invoices (reporting) - CACHED query.
/// </summary>
public record GetPatientInvoicesQuery : ICachedQuery<InvoiceListDto>
{
    public Guid PatientId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"invoices_patient_{PatientId}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get outstanding balance (reporting) - CACHED query.
/// </summary>
public record GetPatientOutstandingBalanceQuery : ICachedQuery<OutstandingBalanceDto>
{
    public Guid PatientId { get; init; }

    public string CacheKey => $"balance_patient_{PatientId}";
    public int CacheDurationSeconds => 300;
}

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
