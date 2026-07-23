using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Identity.Domain.Events;

public record PasswordChangedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime ChangedAt { get; set; }

    public PasswordChangedEvent(Guid id, string email)
    {
        UserId = id;
        Email = email;
        ChangedAt = DateTime.UtcNow;
    }
}
