namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Responses;

/// <summary>
/// Provider availability slots response.
/// </summary>
public class ProviderAvailabilityListDto
{
    public Guid ProviderId { get; set; }
    public List<ProviderAvailabilitySlotDto> Slots { get; set; } = new();
}
