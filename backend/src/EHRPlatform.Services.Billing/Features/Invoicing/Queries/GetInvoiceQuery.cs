using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Billing.Features.Invoicing.Dtos.Responses;

namespace EHRPlatform.Services.Billing.Features.Invoicing.Queries;

/// <summary>
/// Get invoice by ID - CACHED query.
/// </summary>
public record GetInvoiceQuery : ICachedQuery<InvoiceResponseDto>
{
    public Guid InvoiceId { get; init; }

    public string CacheKey => $"invoice_{InvoiceId}";
    public int CacheDurationSeconds => 600;
}
