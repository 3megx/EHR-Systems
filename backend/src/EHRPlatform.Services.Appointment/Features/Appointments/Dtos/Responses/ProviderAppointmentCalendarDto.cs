namespace EHRPlatform.Services.Appointment.Features.Appointments.Dtos.Responses;

/// <summary>
/// Provider appointment calendar for specific date.
/// </summary>
public class ProviderAppointmentCalendarDto
{
    public Guid ProviderId { get; set; }
    public DateTime Date { get; set; }
    public List<AppointmentSlotDto> Slots { get; set; } = new();
}
