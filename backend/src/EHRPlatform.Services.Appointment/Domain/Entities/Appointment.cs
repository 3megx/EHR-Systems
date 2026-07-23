using EHRPlatform.Common.Entities;
using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Domain;

/// <summary>
/// Appointment aggregate root.
/// Manages scheduling, availability, reminders, and cancellations.
/// </summary>
public class Appointment : AuditableEntity
{
    /// <summary>
    /// Gets or sets the patient identifier.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets or sets the provider identifier.
    /// </summary>
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the scheduled start time.
    /// </summary>
    public DateTime ScheduledStart { get; set; }

    /// <summary>
    /// Gets or sets the scheduled end time.
    /// </summary>
    public DateTime ScheduledEnd { get; set; }

    /// <summary>
    /// Gets or sets the appointment type.
    /// Possible values: Office, Telehealth, Phone
    /// </summary>
    public string AppointmentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current appointment status.
    /// Possible values: Scheduled, Confirmed, CheckedIn, Completed, Cancelled, NoShow
    /// </summary>
    public string Status { get; set; } = "Scheduled";

    /// <summary>
    /// Gets or sets the reason for visit.
    /// </summary>
    public string? ReasonForVisit { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the appointment.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the duration of the appointment in minutes.
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a reminder has been sent.
    /// </summary>
    public bool ReminderSent { get; set; }

    /// <summary>
    /// Gets or sets the date and time the appointment was confirmed.
    /// </summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the appointment was cancelled.
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Gets or sets the cancellation reason.
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Gets the collection of reminders for this appointment.
    /// </summary>
    public ICollection<AppointmentReminder> Reminders { get; } = new List<AppointmentReminder>();

    private readonly List<IntegrationEvent> _domainEvents = new();

    /// <summary>
    /// Gets a value indicating whether the appointment is available (scheduled and in the future).
    /// </summary>
    public bool IsAvailable => Status == "Scheduled" && ScheduledStart > DateTime.UtcNow;

    /// <summary>
    /// Confirms the appointment.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if appointment is not scheduled.</exception>
    public void Confirm()
    {
        if (Status != "Scheduled")
            throw new InvalidOperationException("Only scheduled appointments can be confirmed");

        Status = "Confirmed";
        ConfirmedAt = DateTime.UtcNow;
        RaiseEvent(new AppointmentConfirmedEvent(Id, PatientId, ProviderId, ScheduledStart));
    }

    /// <summary>
    /// Cancels the appointment.
    /// </summary>
    /// <param name="reason">Reason for cancellation.</param>
    /// <exception cref="InvalidOperationException">Thrown if appointment is completed or already cancelled.</exception>
    public void Cancel(string reason = "")
    {
        if (Status == "Completed" || Status == "Cancelled")
            throw new InvalidOperationException($"Cannot cancel {Status} appointment");

        Status = "Cancelled";
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        RaiseEvent(new AppointmentCancelledEvent(Id, PatientId, ProviderId, reason));
    }

    /// <summary>
    /// Marks the appointment as checked in.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if appointment is not confirmed.</exception>
    public void CheckIn()
    {
        if (Status != "Confirmed")
            throw new InvalidOperationException("Only confirmed appointments can be checked in");

        Status = "CheckedIn";
        RaiseEvent(new AppointmentCheckedInEvent(Id, PatientId, ProviderId, DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the appointment as completed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if appointment is not checked in.</exception>
    public void Complete()
    {
        if (Status != "CheckedIn")
            throw new InvalidOperationException("Only checked-in appointments can be completed");

        Status = "Completed";
        RaiseEvent(new AppointmentCompletedEvent(Id, PatientId, ProviderId, DateTime.UtcNow));
    }

    /// <summary>
    /// Adds a reminder for this appointment.
    /// </summary>
    /// <param name="reminderTime">The time for the reminder.</param>
    /// <param name="method">The reminder method (Email, SMS, InApp).</param>
    public void AddReminder(DateTime reminderTime, string method = "Email")
    {
        var reminder = new AppointmentReminder
        {
            Id = Guid.NewGuid(),
            AppointmentId = Id,
            ReminderTime = reminderTime,
            Method = method,
            IsSent = false
        };
        Reminders.Add(reminder);
    }

    /// <summary>
    /// Marks a reminder as sent.
    /// </summary>
    /// <param name="reminderId">The reminder identifier.</param>
    public void MarkReminderSent(Guid reminderId)
    {
        var reminder = Reminders.FirstOrDefault(r => r.Id == reminderId);
        if (reminder != null)
            reminder.IsSent = true;
    }

    /// <summary>
    /// Raises a domain event.
    /// </summary>
    /// <param name="event">The domain event to raise.</param>
    public void RaiseEvent(IntegrationEvent @event) => _domainEvents.Add(@event);

    /// <summary>
    /// Gets all raised domain events.
    /// </summary>
    /// <returns>Read-only list of domain events.</returns>
    public IReadOnlyList<IntegrationEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    /// <summary>
    /// Clears all raised domain events.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
