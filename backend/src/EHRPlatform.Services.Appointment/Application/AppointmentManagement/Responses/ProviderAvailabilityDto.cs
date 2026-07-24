namespace EHRPlatform.Services.Appointment.Application.AppointmentManagement.Responses;

/// <summary>
/// Provider availability slot DTO.
/// Returned after creating/updating a provider availability slot.
/// </summary>
public class ProviderAvailabilityDto
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public int? MaxAppointmentsPerSlot { get; set; }
    public int CurrentBookings { get; set; }
    public bool IsActive { get; set; }
}
