namespace EHRPlatform.Services.Notification.Application.NotificationManagement.Responses;

/// <summary>
/// Notification detailed DTO.
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
    public int MaxRetries { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public string? FailureReason { get; set; }
    public string? MessageId { get; set; }
    public Dictionary<string, string> TemplateVars { get; set; } = new();
    public string? Recipient { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
