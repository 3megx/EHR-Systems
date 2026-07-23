using EHRPlatform.Common.Entities;
using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Notification.Features.Notifications.Domain;

/// <summary>
/// Notification aggregate root.
/// Multi-channel delivery: Email, SMS, Push, In-App.
/// </summary>
public class Notification : AuditableEntity
{
    public Guid RecipientId { get; set; }
    public string Channel { get; set; } = string.Empty; // Email, SMS, Push, InApp
    public string NotificationType { get; set; } = string.Empty; // Appointment, Prescription, Billing, Clinical, System
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Sent, Failed, Bounced, Unsubscribed
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; } = 3;
    public DateTime? ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public string? FailureReason { get; set; }
    public string? MessageId { get; set; } // Provider message ID (e.g., SES, Twilio)
    public Dictionary<string, string> TemplateVars { get; set; } = new(); // Template variables
    public string? Recipient { get; set; } // Email address, phone, or device token

    private readonly List<IntegrationEvent> _domainEvents = new();

    public void MarkSent(string messageId = "")
    {
        Status = "Sent";
        SentAt = DateTime.UtcNow;
        MessageId = messageId;
        RetryCount = 0;

        RaiseEvent(new NotificationSentEvent(Id, RecipientId, Channel, NotificationType));
    }

    public void MarkFailed(string reason)
    {
        RetryCount++;
        if (RetryCount >= MaxRetries)
        {
            Status = "Failed";
            FailureReason = reason;
            RaiseEvent(new NotificationFailedEvent(Id, RecipientId, Channel, reason));
        }
        else
        {
            // Retry with exponential backoff
            ScheduledFor = DateTime.UtcNow.AddSeconds(Math.Pow(2, RetryCount));
        }
    }

    public void MarkBounced()
    {
        Status = "Bounced";
        RaiseEvent(new NotificationBouncedEvent(Id, RecipientId, Channel));
    }

    public void MarkUnsubscribed()
    {
        Status = "Unsubscribed";
        RaiseEvent(new NotificationUnsubscribedEvent(Id, RecipientId, Channel));
    }

    public void RaiseEvent(IntegrationEvent @event) => _domainEvents.Add(@event);
    public IReadOnlyList<IntegrationEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>
/// Notification template for reusable messages.
/// </summary>
public class NotificationTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty; // Email, SMS, Push
    public string NotificationType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty; // With {{variable}} placeholders
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Render template with variables.
    /// </summary>
    public string RenderBody(Dictionary<string, string> variables)
    {
        var body = BodyTemplate;
        foreach (var (key, value) in variables)
        {
            body = body.Replace($"{{{{{key}}}}}", value ?? "");
        }
        return body;
    }
}

/// <summary>
/// User notification preferences (opt-in/out).
/// </summary>
public class NotificationPreference : BaseEntity
{
    public Guid UserId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// Domain events.
/// </summary>
public record NotificationCreatedEvent : IntegrationEvent
{
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    public string Channel { get; set; }
    public string NotificationType { get; set; }

    public NotificationCreatedEvent(Guid id, Guid recipientId, string channel, string type)
    {
        NotificationId = id;
        RecipientId = recipientId;
        Channel = channel;
        NotificationType = type;
    }
}

public record NotificationSentEvent : IntegrationEvent
{
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    public string Channel { get; set; }
    public string NotificationType { get; set; }

    public NotificationSentEvent(Guid id, Guid recipientId, string channel, string type)
    {
        NotificationId = id;
        RecipientId = recipientId;
        Channel = channel;
        NotificationType = type;
    }
}

public record NotificationFailedEvent : IntegrationEvent
{
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    public string Channel { get; set; }
    public string Reason { get; set; }

    public NotificationFailedEvent(Guid id, Guid recipientId, string channel, string reason)
    {
        NotificationId = id;
        RecipientId = recipientId;
        Channel = channel;
        Reason = reason;
    }
}

public record NotificationBouncedEvent : IntegrationEvent
{
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    public string Channel { get; set; }

    public NotificationBouncedEvent(Guid id, Guid recipientId, string channel)
    {
        NotificationId = id;
        RecipientId = recipientId;
        Channel = channel;
    }
}

public record NotificationUnsubscribedEvent : IntegrationEvent
{
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    public string Channel { get; set; }

    public NotificationUnsubscribedEvent(Guid id, Guid recipientId, string channel)
    {
        NotificationId = id;
        RecipientId = recipientId;
        Channel = channel;
    }
}
