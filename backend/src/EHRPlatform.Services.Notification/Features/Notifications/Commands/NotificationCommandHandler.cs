using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Messaging;
using EHRPlatform.Services.Notification.Features.Notifications.Domain;
using Mapster;

namespace EHRPlatform.Services.Notification.Features.Notifications.Commands;

/// <summary>
/// Send notification handler.
/// Routes to appropriate channel provider (email, SMS, push, in-app).
/// </summary>
public class SendNotificationCommandHandler : ICommandHandler<SendNotificationCommand, NotificationResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<SendNotificationCommandHandler> _logger;

    public SendNotificationCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<SendNotificationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task<NotificationResponseDto> Handle(
        SendNotificationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Sending notification to {RecipientId} via {Channel}",
            command.RecipientId, command.Channel);

        // Check user preferences
        var prefRepo = _unitOfWork.Repository<NotificationPreference>();
        var preference = await prefRepo.FirstOrDefaultAsync(
            q => q.Where(p =>
                p.UserId == command.RecipientId &&
                p.Channel == command.Channel &&
                p.NotificationType == command.NotificationType),
            cancellationToken);

        if (preference?.IsEnabled == false)
            throw new InvalidOperationException("User has disabled this notification type");

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            RecipientId = command.RecipientId,
            Channel = command.Channel,
            NotificationType = command.NotificationType,
            Subject = command.Subject,
            Body = command.Body,
            Recipient = command.Recipient,
            ScheduledFor = command.ScheduledFor ?? DateTime.UtcNow,
            TemplateVars = command.TemplateVars ?? new()
        };

        var repo = _unitOfWork.Repository<Notification>();
        await repo.AddAsync(notification, cancellationToken);

        // Publish event
        var createdEvent = new NotificationCreatedEvent(
            notification.Id, notification.RecipientId, notification.Channel, notification.NotificationType);

        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = notification.Id,
            EventType = nameof(NotificationCreatedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(createdEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Notification created {NotificationId}", notification.Id);

        return notification.Adapt<NotificationResponseDto>();
    }
}

/// <summary>
/// Mark notification sent handler.
/// </summary>
public class MarkNotificationSentCommandHandler : ICommandHandler<MarkNotificationSentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<MarkNotificationSentCommandHandler> _logger;

    public MarkNotificationSentCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<MarkNotificationSentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(MarkNotificationSentCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking notification {NotificationId} as sent", command.NotificationId);

        var repo = _unitOfWork.Repository<Notification>();
        var notification = await repo.FirstOrDefaultAsync(
            q => q.Where(n => n.Id == command.NotificationId),
            cancellationToken);

        if (notification == null)
            throw new InvalidOperationException($"Notification {command.NotificationId} not found");

        notification.MarkSent(command.MessageId ?? "");
        await repo.UpdateAsync(notification, cancellationToken);

        // Publish event
        var sentEvent = notification.GetDomainEvents().Last();
        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = notification.Id,
            EventType = nameof(NotificationSentEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(sentEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Mark notification failed handler.
/// </summary>
public class MarkNotificationFailedCommandHandler : ICommandHandler<MarkNotificationFailedCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly ILogger<MarkNotificationFailedCommandHandler> _logger;

    public MarkNotificationFailedCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        ILogger<MarkNotificationFailedCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    public async Task Handle(MarkNotificationFailedCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking notification {NotificationId} as failed: {Reason}",
            command.NotificationId, command.Reason);

        var repo = _unitOfWork.Repository<Notification>();
        var notification = await repo.FirstOrDefaultAsync(
            q => q.Where(n => n.Id == command.NotificationId),
            cancellationToken);

        if (notification == null)
            throw new InvalidOperationException($"Notification {command.NotificationId} not found");

        notification.MarkFailed(command.Reason);
        await repo.UpdateAsync(notification, cancellationToken);

        // Publish event if finally failed
        if (notification.Status == "Failed")
        {
            var failedEvent = notification.GetDomainEvents().Last();
            await _outbox.AddAsync(new OutboxEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = notification.Id,
                EventType = nameof(NotificationFailedEvent),
                EventData = System.Text.Json.JsonSerializer.Serialize(failedEvent),
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Set notification preference handler.
/// </summary>
public class SetNotificationPreferenceCommandHandler : ICommandHandler<SetNotificationPreferenceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SetNotificationPreferenceCommandHandler> _logger;

    public SetNotificationPreferenceCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<SetNotificationPreferenceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(SetNotificationPreferenceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Setting preference for user {UserId}: {Channel}/{Type} = {Enabled}",
            command.UserId, command.Channel, command.NotificationType, command.IsEnabled);

        var prefRepo = _unitOfWork.Repository<NotificationPreference>();
        var preference = await prefRepo.FirstOrDefaultAsync(
            q => q.Where(p =>
                p.UserId == command.UserId &&
                p.Channel == command.Channel &&
                p.NotificationType == command.NotificationType),
            cancellationToken);

        if (preference == null)
        {
            preference = new NotificationPreference
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                Channel = command.Channel,
                NotificationType = command.NotificationType,
                IsEnabled = command.IsEnabled
            };
            await prefRepo.AddAsync(preference, cancellationToken);
        }
        else
        {
            preference.IsEnabled = command.IsEnabled;
            await prefRepo.UpdateAsync(preference, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
