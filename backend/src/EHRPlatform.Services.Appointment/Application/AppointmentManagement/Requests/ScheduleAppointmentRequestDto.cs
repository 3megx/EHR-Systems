namespace EHRPlatform.Services.Appointment.Application.AppointmentManagement.Requests;

/// <summary>
/// Schedule appointment request DTO.
/// </summary>
public class ScheduleAppointmentRequestDto
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public int DurationMinutes { get; set; }
    public string AppointmentType { get; set; } = string.Empty;
    public string? ReasonForVisit { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Set provider availability request DTO.
/// </summary>
public class SetProviderAvailabilityRequestDto
{
    public Guid ProviderId { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public int? MaxAppointmentsPerSlot { get; set; }
}
