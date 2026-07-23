namespace EHRPlatform.Services.Appointment.Features.Appointments.Dtos.Responses;

/// <summary>
/// Appointment command/update DTO.
/// Used for command submissions (not including computed fields).
/// </summary>
public class AppointmentCommandDto
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
}
