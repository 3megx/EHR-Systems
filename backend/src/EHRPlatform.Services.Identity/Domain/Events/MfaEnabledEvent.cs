using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Identity.Domain.Events;

public record MfaEnabledEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string MfaType { get; set; }

    public MfaEnabledEvent(Guid id, string email, string type)
    {
        UserId = id;
        Email = email;
        MfaType = type;
    }
}
