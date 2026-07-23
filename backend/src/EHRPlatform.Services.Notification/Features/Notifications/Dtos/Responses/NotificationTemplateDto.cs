namespace EHRPlatform.Services.Notification.Features.Notifications.Dtos.Responses;

/// <summary>
/// Notification template DTO.
/// Single Responsibility: Represent notification template and user preferences.
/// </summary>
public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>
/// User preference DTO.
/// </summary>
public class PreferenceDto
{
    public string Channel { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
