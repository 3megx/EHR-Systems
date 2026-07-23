using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Notification.Domain.Events;

/// <summary>
/// Domain event raised when a notification is created.
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
