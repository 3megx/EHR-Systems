namespace EHRPlatform.Services.Appointment.Application.AppointmentBooking.Responses;

public class ProviderAvailabilitySlotDto
{
    public Guid Id { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public int? MaxAppointmentsPerSlot { get; set; }
    public int CurrentBookings { get; set; }
    public bool HasAvailability { get; set; }
}
