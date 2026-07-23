namespace EHRPlatform.Services.Notification.Application.NotificationManagement.Responses;

/// <summary>
/// Notification template DTO.
/// </summary>
public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
