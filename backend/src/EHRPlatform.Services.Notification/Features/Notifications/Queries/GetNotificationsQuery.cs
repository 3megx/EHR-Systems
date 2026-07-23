using MediatR;

namespace EHRPlatform.Services.Notification.Features.Notifications.Queries;

public class GetNotificationsQuery : IRequest<IEnumerable<object>>
{
    public Guid RecipientId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
