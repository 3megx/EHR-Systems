using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Notification.Features.Notifications.Commands;

/// <summary>
/// Send notification command.
/// </summary>
public record SendNotificationCommand : ICommand<NotificationResponseDto>
{
    public Guid RecipientId { get; init; }
    public string Channel { get; init; } = string.Empty; // Email, SMS, Push, InApp
    public string NotificationType { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public Dictionary<string, string>? TemplateVars { get; init; }
    public string? Recipient { get; init; } // Email, phone, device token
    public DateTime? ScheduledFor { get; init; }
}

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.RecipientId).NotEmpty();
        RuleFor(x => x.Channel).Must(c => new[] { "Email", "SMS", "Push", "InApp" }.Contains(c));
        RuleFor(x => x.NotificationType).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Body).NotEmpty();
    }
}

/// <summary>
/// Mark notification sent command.
/// </summary>
public record MarkNotificationSentCommand : ICommand
{
    public Guid NotificationId { get; init; }
    public string? MessageId { get; init; }
}

/// <summary>
/// Mark notification failed command.
/// </summary>
public record MarkNotificationFailedCommand : ICommand
{
    public Guid NotificationId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Set notification preference command.
/// </summary>
public record SetNotificationPreferenceCommand : ICommand
{
    public Guid UserId { get; init; }
    public string Channel { get; init; } = string.Empty;
    public string NotificationType { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
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
    public DateTime? ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public string? FailureReason { get; set; }
    public string? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
}
