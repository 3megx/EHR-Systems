using EHRPlatform.Services.Notification.Features.Notifications.Dtos.Responses;

namespace EHRPlatform.Services.Notification.Application.NotificationManagement.Responses;

/// <summary>
/// Notification list DTO with pagination.
/// </summary>
public class NotificationListDto
{
    public List<NotificationResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
