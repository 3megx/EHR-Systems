namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Requests;

/// <summary>
/// Check-in appointment request DTO.
/// </summary>
public class CheckInAppointmentRequest
{
    public Guid AppointmentId { get; set; }
}
