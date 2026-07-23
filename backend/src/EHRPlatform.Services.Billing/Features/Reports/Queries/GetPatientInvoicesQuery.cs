using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Billing.Application.Invoicing.Responses;

namespace EHRPlatform.Services.Billing.Features.Reports.Queries;

/// <summary>
/// Get patient invoices - CACHED query.
/// </summary>
public record GetPatientInvoicesQuery : ICachedQuery<InvoiceListDto>
{
    public Guid PatientId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"invoices_patient_{PatientId}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 600;
}

