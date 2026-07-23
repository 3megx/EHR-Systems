namespace EHRPlatform.Services.Notification.Features.Notifications.Dtos.Responses;

/// <summary>
/// Notification list DTO.
/// Single Responsibility: Represent notifications in paginated responses.
/// </summary>
public class NotificationListDto
{
    public List<NotificationResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
