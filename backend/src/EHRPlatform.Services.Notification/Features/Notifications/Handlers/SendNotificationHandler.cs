using MediatR;
using EHRPlatform.Services.Notification.Features.Notifications.Commands;

namespace EHRPlatform.Services.Notification.Features.Notifications.Handlers;

public class SendNotificationHandler : IRequestHandler<SendNotificationCommand, object>
{
    public Task<object> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class SendNotificationCommand : IRequest<object>
{
    public Guid RecipientId { get; set; }
    public string? Channel { get; set; }
    public string? Message { get; set; }
}
