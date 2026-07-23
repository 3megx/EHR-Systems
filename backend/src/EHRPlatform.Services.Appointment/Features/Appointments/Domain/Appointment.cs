using EHRPlatform.Common.Entities;
using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Domain;

/// <summary>
/// Appointment aggregate root.
/// Manages scheduling, availability, reminders, cancellations.
/// </summary>
public class Appointment : AuditableEntity
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    public string AppointmentType { get; set; } = string.Empty; // Office, Telehealth, Phone
    public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, CheckedIn, Completed, Cancelled, NoShow
    public string? ReasonForVisit { get; set; }
    public string? Notes { get; set; }
    public int DurationMinutes { get; set; }
    public bool ReminderSent { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    // Collections
    public ICollection<AppointmentReminder> Reminders { get; } = new List<AppointmentReminder>();

    private readonly List<IntegrationEvent> _domainEvents = new();

    public bool IsAvailable => Status == "Scheduled" && ScheduledStart > DateTime.UtcNow;

    public void Confirm()
    {
        if (Status != "Scheduled")
            throw new InvalidOperationException("Only scheduled appointments can be confirmed");

        Status = "Confirmed";
        ConfirmedAt = DateTime.UtcNow;
        RaiseEvent(new AppointmentConfirmedEvent(Id, PatientId, ProviderId, ScheduledStart));
    }

    public void Cancel(string reason = "")
    {
        if (Status == "Completed" || Status == "Cancelled")
            throw new InvalidOperationException($"Cannot cancel {Status} appointment");

        Status = "Cancelled";
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        RaiseEvent(new AppointmentCancelledEvent(Id, PatientId, ProviderId, reason));
    }

    public void CheckIn()
    {
        if (Status != "Confirmed")
            throw new InvalidOperationException("Only confirmed appointments can be checked in");

        Status = "CheckedIn";
        RaiseEvent(new AppointmentCheckedInEvent(Id, PatientId, ProviderId, DateTime.UtcNow));
    }

    public void Complete()
    {
        if (Status != "CheckedIn")
            throw new InvalidOperationException("Only checked-in appointments can be completed");

        Status = "Completed";
        RaiseEvent(new AppointmentCompletedEvent(Id, PatientId, ProviderId, DateTime.UtcNow));
    }

    public void AddReminder(DateTime reminderTime, string method = "Email")
    {
        var reminder = new AppointmentReminder
        {
            Id = Guid.NewGuid(),
            AppointmentId = Id,
            ReminderTime = reminderTime,
            Method = method, // Email, SMS, InApp
            IsSent = false
        };
        Reminders.Add(reminder);
    }

    public void MarkReminderSent(Guid reminderId)
    {
        var reminder = Reminders.FirstOrDefault(r => r.Id == reminderId);
        if (reminder != null)
            reminder.IsSent = true;
    }

    public void RaiseEvent(IntegrationEvent @event) => _domainEvents.Add(@event);
    public IReadOnlyList<IntegrationEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>
/// Appointment reminder notification.
/// </summary>
public class AppointmentReminder : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public DateTime ReminderTime { get; set; }
    public string Method { get; set; } = string.Empty; // Email, SMS, InApp
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
    public Appointment Appointment { get; set; } = null!;
}

/// <summary>
/// Provider availability slot.
/// Recurring or one-time slots.
/// </summary>
public class ProviderAvailability : BaseEntity
{
    public Guid ProviderId { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; } // Daily, Weekly, Monthly
    public int? MaxAppointmentsPerSlot { get; set; }
    public int CurrentBookings { get; set; }
    public bool IsActive { get; set; } = true;

    public bool HasAvailability() =>
        MaxAppointmentsPerSlot == null || CurrentBookings < MaxAppointmentsPerSlot.Value;

    public void BookSlot() => CurrentBookings++;
    public void ReleaseSlot() => CurrentBookings = Math.Max(0, CurrentBookings - 1);
}

/// <summary>
/// Domain events.
/// </summary>
public record AppointmentScheduledEvent : IntegrationEvent
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public string AppointmentType { get; set; }

    public AppointmentScheduledEvent(Guid id, Guid patientId, Guid providerId, DateTime start, string type)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        ScheduledStart = start;
        AppointmentType = type;
    }
}

public record AppointmentConfirmedEvent : IntegrationEvent
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime ScheduledStart { get; set; }

    public AppointmentConfirmedEvent(Guid id, Guid patientId, Guid providerId, DateTime start)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        ScheduledStart = start;
    }
}

public record AppointmentCancelledEvent : IntegrationEvent
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public string Reason { get; set; }

    public AppointmentCancelledEvent(Guid id, Guid patientId, Guid providerId, string reason)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        Reason = reason;
    }
}

public record AppointmentCheckedInEvent : IntegrationEvent
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime CheckInTime { get; set; }

    public AppointmentCheckedInEvent(Guid id, Guid patientId, Guid providerId, DateTime checkIn)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        CheckInTime = checkIn;
    }
}

public record AppointmentCompletedEvent : IntegrationEvent
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime CompletedAt { get; set; }

    public AppointmentCompletedEvent(Guid id, Guid patientId, Guid providerId, DateTime completed)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        CompletedAt = completed;
    }
}
