using EHRPlatform.Common.CQRS;

namespace EHRPlatform.Services.Notification.Features.Notifications.Queries;

/// <summary>
/// Get notification by ID - CACHED query.
/// </summary>
public record GetNotificationQuery : ICachedQuery<NotificationResponseDto>
{
    public Guid NotificationId { get; init; }

    public string CacheKey => $"notification_{NotificationId}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get user notifications - CACHED query.
/// </summary>
public record GetUserNotificationsQuery : ICachedQuery<NotificationListDto>
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"notifications_user_{UserId}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 300;
}

/// <summary>
/// Get user notification preferences - CACHED query.
/// </summary>
public record GetUserPreferencesQuery : ICachedQuery<List<PreferenceDto>>
{
    public Guid UserId { get; init; }

    public string CacheKey => $"preferences_user_{UserId}";
    public int CacheDurationSeconds => 900;
}

/// <summary>
/// Notification list DTO.
/// </summary>
public class NotificationListDto
{
    public List<NotificationResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Notification response DTO.
/// </summary>
public class NotificationResponseDto
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
}

/// <summary>
/// Preference DTO.
/// </summary>
public class PreferenceDto
{
    public string Channel { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
