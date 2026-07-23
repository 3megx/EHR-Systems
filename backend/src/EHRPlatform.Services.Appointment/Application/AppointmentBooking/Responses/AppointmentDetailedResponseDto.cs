namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Responses;

/// <summary>
/// Appointment detailed response DTO.
/// Includes computed fields and enriched data.
/// </summary>
public class AppointmentDetailedResponseDto
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
    public List<AppointmentReminderDto> Reminders { get; set; } = new();
    public bool IsAvailable { get; set; }
    public double TimeUntilAppointment { get; set; } // Minutes
}
