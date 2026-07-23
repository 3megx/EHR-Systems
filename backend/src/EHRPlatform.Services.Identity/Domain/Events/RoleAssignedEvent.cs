using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Identity.Domain.Events;

public record RoleAssignedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
    public string Email { get; set; }

    public RoleAssignedEvent(Guid userId, Guid roleId, string roleName, string email)
    {
        UserId = userId;
        RoleId = roleId;
        RoleName = roleName;
        Email = email;
    }
}
