namespace EHRPlatform.Services.Notification.Application.Notifications.Requests;

public class SendNotificationRequest
{
    public Guid RecipientId { get; set; }
    public string? Channel { get; set; }
    public string? NotificationType { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
}
