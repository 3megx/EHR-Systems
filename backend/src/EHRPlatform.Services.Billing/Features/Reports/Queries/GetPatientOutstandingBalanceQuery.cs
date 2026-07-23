using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Billing.Application.Reports.Responses;

namespace EHRPlatform.Services.Billing.Features.Reports.Queries;

/// <summary>
/// Get outstanding balance - CACHED query.
/// Retrieves comprehensive balance information for a patient.
/// </summary>
public record GetPatientOutstandingBalanceQuery : ICachedQuery<OutstandingBalanceDto>
{
    public Guid PatientId { get; init; }

    public string CacheKey => $"balance_patient_{PatientId}";
    public int CacheDurationSeconds => 300;
}
