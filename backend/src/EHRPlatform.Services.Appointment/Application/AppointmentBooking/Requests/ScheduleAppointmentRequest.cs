namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Requests;

/// <summary>
/// Schedule appointment request DTO.
/// </summary>
public class ScheduleAppointmentRequest
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public int DurationMinutes { get; set; }
    public string AppointmentType { get; set; } = string.Empty; // Office, Telehealth, Phone
    public string? ReasonForVisit { get; set; }
    public string? Notes { get; set; }
}
