namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Requests;

/// <summary>
/// Complete appointment request DTO.
/// </summary>
public class CompleteAppointmentRequest
{
    public Guid AppointmentId { get; set; }
}
