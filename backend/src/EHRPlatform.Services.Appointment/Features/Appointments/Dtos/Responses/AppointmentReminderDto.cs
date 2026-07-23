namespace EHRPlatform.Services.Appointment.Features.Appointments.Dtos.Responses;

/// <summary>
/// Appointment reminder DTO.
/// </summary>
public class AppointmentReminderDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public DateTime ReminderDateTime { get; set; }
    public string Channel { get; set; } = string.Empty; // Email, SMS, Push
    public string Status { get; set; } = string.Empty; // Scheduled, Sent, Failed
    public DateTime? SentAt { get; set; }
}
