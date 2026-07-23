namespace EHRPlatform.Services.Appointment.Features.Appointments.Dtos.Responses;

public class AppointmentSlotDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Status { get; set; } = string.Empty; // Available, Booked, Blocked
    public Guid? AppointmentId { get; set; }
    public Guid? PatientId { get; set; }
}
