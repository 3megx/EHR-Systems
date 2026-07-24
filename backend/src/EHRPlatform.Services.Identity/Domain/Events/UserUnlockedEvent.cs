using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Identity.Domain.Events;

public class UserUnlockedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;

    public UserUnlockedEvent() { }

    public UserUnlockedEvent(Guid id, string email)
    {
        UserId = id;
        Email  = email;
    }
}
