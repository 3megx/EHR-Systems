namespace EHRPlatform.Services.Appointment.Application.Appointments.Responses;

public class AppointmentResponse
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    public string? Status { get; set; }
    public string? AppointmentType { get; set; }
}
