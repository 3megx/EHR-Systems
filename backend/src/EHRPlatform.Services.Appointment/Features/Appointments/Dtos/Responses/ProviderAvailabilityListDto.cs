namespace EHRPlatform.Services.Appointment.Features.Appointments.Dtos.Responses;

/// <summary>
/// Provider availability slots response.
/// </summary>
public class ProviderAvailabilityListDto
{
    public Guid ProviderId { get; set; }
    public List<ProviderAvailabilitySlotDto> Slots { get; set; } = new();
}
