using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Analytics.Domain.Entities;

/// <summary>
/// Dashboard widget (chart, table, KPI card).
/// </summary>
public class DashboardWidget : BaseEntity
{
    public Guid DashboardId { get; set; }
    public string WidgetType { get; set; } = string.Empty; // LineChart, BarChart, KPI, Table
    public string Title { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public int Position { get; set; }
    public int SizeX { get; set; }
    public int SizeY { get; set; }
    public Dictionary<string, string> Configuration { get; set; } = new(); // Chart options
    public Dashboard Dashboard { get; set; } = null!;
}
