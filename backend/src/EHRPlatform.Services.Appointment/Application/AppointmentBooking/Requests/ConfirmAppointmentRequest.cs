namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Requests;

/// <summary>
/// Confirm appointment request DTO.
/// </summary>
public class ConfirmAppointmentRequest
{
    public Guid AppointmentId { get; set; }
}
