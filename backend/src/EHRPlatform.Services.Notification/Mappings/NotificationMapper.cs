using Mapster;
using EHRPlatform.Common.Mapping;
using EHRPlatform.Services.Notification.Features.Notifications.Domain;
using EHRPlatform.Services.Notification.Features.Notifications.Queries;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Notification.Mappings;

/// <summary>
/// Notification Mapper
/// Single Responsibility: Convert between Notification domain models and DTOs.
/// Handles all Notification-related mappings with optional post-processing.
/// </summary>
public class NotificationMapper : MappingServiceBase<Notification, NotificationResponseDto>
{
    public NotificationMapper(ILogger<NotificationMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single notification to response DTO.
    /// </summary>
    public NotificationResponseDto MapToResponseDto(Notification notification)
    {
        return MapToDto(notification);
    }

    /// <summary>
    /// Map notification to detailed DTO.
    /// </summary>
    public NotificationDetailedDto MapToDetailedDto(Notification notification)
    {
        Logger.LogDebug("Mapping notification {NotificationId} to detailed DTO", notification.Id);

        return new NotificationDetailedDto
        {
            Id = notification.Id,
            RecipientId = notification.RecipientId,
            Channel = notification.Channel,
            NotificationType = notification.NotificationType,
            Subject = notification.Subject,
            Body = notification.Body,
            Status = notification.Status,
            RetryCount = notification.RetryCount,
            MaxRetries = notification.MaxRetries,
            ScheduledFor = notification.ScheduledFor,
            SentAt = notification.SentAt,
            FailureReason = notification.FailureReason,
            MessageId = notification.MessageId,
            TemplateVars = notification.TemplateVars,
            Recipient = notification.Recipient,
            CreatedAt = notification.CreatedAt,
            LastModifiedAt = notification.LastModifiedAt
        };
    }

    /// <summary>
    /// Map collection of notifications to paginated DTO.
    /// </summary>
    public NotificationListDto MapToListDto(
        ICollection<Notification> notifications,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} notifications to paginated list DTO", notifications.Count);

        return new NotificationListDto
        {
            Items = notifications.Adapt<List<NotificationResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Map collection of notifications to response DTO list.
    /// </summary>
    public List<NotificationResponseDto> MapToResponseDtoList(ICollection<Notification> notifications)
    {
        Logger.LogDebug("Mapping {Count} notifications to response DTO list", notifications.Count);
        return notifications.Adapt<List<NotificationResponseDto>>();
    }

    /// <summary>
    /// Map notification template to DTO.
    /// </summary>
    public NotificationTemplateDto MapTemplateToDto(NotificationTemplate template)
    {
        Logger.LogDebug("Mapping notification template {TemplateId} to DTO", template.Id);

        return new NotificationTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Channel = template.Channel,
            NotificationType = template.NotificationType,
            Subject = template.Subject,
            BodyTemplate = template.BodyTemplate,
            IsActive = template.IsActive
        };
    }

    /// <summary>
    /// Map collection of templates to DTO list.
    /// </summary>
    public List<NotificationTemplateDto> MapTemplatesToDtoList(ICollection<NotificationTemplate> templates)
    {
        Logger.LogDebug("Mapping {Count} notification templates to DTO list", templates.Count);
        return templates.Adapt<List<NotificationTemplateDto>>();
    }
}

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
