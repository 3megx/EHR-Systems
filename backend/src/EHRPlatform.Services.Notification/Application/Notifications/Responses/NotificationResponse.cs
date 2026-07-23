namespace EHRPlatform.Services.Notification.Application.Notifications.Responses;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public Guid RecipientId { get; set; }
    public string? Channel { get; set; }
    public string? NotificationType { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
