using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Identity.Domain.Events;

public record UserUnlockedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }

    public UserUnlockedEvent(Guid id, string email)
    {
        UserId = id;
        Email = email;
    }
}
