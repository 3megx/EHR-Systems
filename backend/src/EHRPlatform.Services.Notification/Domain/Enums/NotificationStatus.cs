namespace EHRPlatform.Services.Notification.Domain.Enums;

public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3,
    Bounced = 4,
    Unsubscribed = 5,
    Retrying = 6
}
