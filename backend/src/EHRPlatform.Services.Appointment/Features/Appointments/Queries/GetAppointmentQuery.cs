using EHRPlatform.Common.CQRS;

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

/// <summary>
/// Appointment list response.
/// </summary>
public class AppointmentListDto
{
    public List<AppointmentResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Appointment response DTO.
/// </summary>
public class AppointmentResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    public string AppointmentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReasonForVisit { get; set; }
    public string? Notes { get; set; }
    public int DurationMinutes { get; set; }
    public bool ReminderSent { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
}

/// <summary>
/// Provider appointment calendar for specific date.
/// </summary>
public class ProviderAppointmentCalendarDto
{
    public Guid ProviderId { get; set; }
    public DateTime Date { get; set; }
    public List<AppointmentSlotDto> Slots { get; set; } = new();
}

public class AppointmentSlotDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Status { get; set; } = string.Empty; // Available, Booked, Blocked
    public Guid? AppointmentId { get; set; }
    public Guid? PatientId { get; set; }
}

/// <summary>
/// Provider availability slots response.
/// </summary>
public class ProviderAvailabilityListDto
{
    public Guid ProviderId { get; set; }
    public List<ProviderAvailabilitySlotDto> Slots { get; set; } = new();
}

public class ProviderAvailabilitySlotDto
{
    public Guid Id { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public int? MaxAppointmentsPerSlot { get; set; }
    public int CurrentBookings { get; set; }
    public bool HasAvailability { get; set; }
}
