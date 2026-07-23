namespace EHRPlatform.Services.Notification.Features.Notifications.Dtos.Responses;

/// <summary>
/// Detailed notification DTO.
/// Single Responsibility: Represent complete notification with template and metadata.
/// </summary>
public class NotificationDetailedDto
{
    public Guid Id { get; set; }
    public Guid RecipientId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public DateTime? SentAt { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public NotificationTemplateDto? Template { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}
