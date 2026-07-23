namespace EHRPlatform.Services.Notification.Application.NotificationManagement.Requests;

/// <summary>
/// Send notification request DTO.
/// Single Responsibility: Accept notification parameters from API layer.
/// </summary>
public class SendNotificationRequest
{
    public Guid RecipientId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string>? TemplateVars { get; set; }
    public string? Recipient { get; set; }
    public DateTime? ScheduledFor { get; set; }
}

/// <summary>
/// Set notification preference request DTO.
/// </summary>
public class SetNotificationPreferenceRequest
{
    public Guid UserId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
