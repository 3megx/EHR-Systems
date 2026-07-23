namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Requests;

/// <summary>
/// Set provider availability request DTO.
/// </summary>
public class SetProviderAvailabilityRequest
{
    public Guid ProviderId { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; } // Daily, Weekly, Monthly
    public int? MaxAppointmentsPerSlot { get; set; }
}
