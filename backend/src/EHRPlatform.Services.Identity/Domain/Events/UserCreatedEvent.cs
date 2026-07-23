using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Identity.Domain.Events;

public record UserCreatedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public UserCreatedEvent(Guid id, string email, string firstName, string lastName)
    {
        UserId = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}
