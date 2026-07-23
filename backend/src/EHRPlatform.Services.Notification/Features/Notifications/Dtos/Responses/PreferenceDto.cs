namespace EHRPlatform.Services.Notification.Features.Notifications.Dtos.Responses;

/// <summary>
/// User notification preference DTO.
/// </summary>
public class PreferenceDto
{
    public string Channel { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
