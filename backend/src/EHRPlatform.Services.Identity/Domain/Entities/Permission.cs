using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Identity.Domain.Entities;

/// <summary>
/// Permission for fine-grained access control.
/// Format: "resource:action" (e.g., "patient:read", "patient:write")
/// </summary>
public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
