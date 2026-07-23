using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Identity.Domain.Events;

public record UserLockedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Reason { get; set; }

    public UserLockedEvent(Guid id, string email, string reason = "Failed login attempts")
    {
        UserId = id;
        Email = email;
        Reason = reason;
    }
}
