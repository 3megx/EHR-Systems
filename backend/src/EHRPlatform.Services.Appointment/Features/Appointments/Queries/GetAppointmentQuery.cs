using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Appointment.Features.Appointments.Dtos.Responses;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Queries;

/// <summary>
/// Get appointment by ID - CACHED query.
/// </summary>
public record GetAppointmentQuery : ICachedQuery<AppointmentResponseDto>
{
    public Guid AppointmentId { get; init; }

    public string CacheKey => $"appointment_{AppointmentId}";
    public int CacheDurationSeconds => 600; // 10 minutes
}

/// <summary>
/// Get patient appointments.
/// Paginated, optional date range filter, CACHED.
/// </summary>
public record GetPatientAppointmentsQuery : ICachedQuery<AppointmentListDto>
{
    public Guid PatientId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public string CacheKey => $"appointments_patient_{PatientId}_{FromDate:yyyyMMdd}_{ToDate:yyyyMMdd}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get provider appointments.
/// Calendar view for scheduling.
/// CACHED.
/// </summary>
public record GetProviderAppointmentsQuery : ICachedQuery<ProviderAppointmentCalendarDto>
{
    public Guid ProviderId { get; init; }
    public DateTime Date { get; init; }

    public string CacheKey => $"appointments_provider_{ProviderId}_{Date:yyyyMMdd}";
    public int CacheDurationSeconds => 300; // 5 minutes - shorter for real-time calendar
}

/// <summary>
/// Get provider availability slots.
/// CACHED.
/// </summary>
public record GetProviderAvailabilityQuery : ICachedQuery<ProviderAvailabilityListDto>
{
    public Guid ProviderId { get; init; }
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }

    public string CacheKey => $"availability_{ProviderId}_{FromDate:yyyyMMdd}_{ToDate:yyyyMMdd}";
    public int CacheDurationSeconds => 300;
}
