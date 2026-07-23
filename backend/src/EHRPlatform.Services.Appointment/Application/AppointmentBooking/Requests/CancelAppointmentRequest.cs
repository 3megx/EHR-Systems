namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Requests;

/// <summary>
/// Cancel appointment request DTO.
/// </summary>
public class CancelAppointmentRequest
{
    public Guid AppointmentId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
