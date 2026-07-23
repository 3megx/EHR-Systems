using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Analytics.Domain.Entities;

/// <summary>
/// Dashboard configuration for users.
/// </summary>
public class Dashboard : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public List<string> Widgets { get; set; } = new(); // Widget IDs/names to display

    public ICollection<DashboardWidget> DashboardWidgets { get; } = new List<DashboardWidget>();
}
