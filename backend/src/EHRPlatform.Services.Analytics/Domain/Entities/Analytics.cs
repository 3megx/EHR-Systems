using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Analytics.Features.Analytics.Domain;

/// <summary>
/// Analytics metric aggregation.
/// Tracks KPIs: patient volume, appointments, revenue, etc.
/// </summary>
public class AnalyticsMetric : BaseEntity
{
    public string MetricName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Patients, Appointments, Revenue, Clinical
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty; // count, USD, percentage
    public Dictionary<string, string> Dimensions { get; set; } = new(); // provider, department, status
    public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly
}

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

/// <summary>
/// Report template for scheduled generation.
/// </summary>
public class Report : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty; // Clinical, Financial, Operational
    public List<string> Metrics { get; set; } = new();
    public string Schedule { get; set; } = string.Empty; // Daily, Weekly, Monthly, OnDemand
    public DateTime? LastGeneratedAt { get; set; }
    public string? LastGeneratedPath { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ReportExecution> Executions { get; } = new List<ReportExecution>();
}

/// <summary>
/// Report execution record.
/// </summary>
public class ReportExecution : BaseEntity
{
    public Guid ReportId { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Running, Completed, Failed
    public string? OutputPath { get; set; }
    public long? FileSize { get; set; }
    public string? ErrorMessage { get; set; }
    public int RecordCount { get; set; }
    public Report Report { get; set; } = null!;
}

/// <summary>
/// Event metrics aggregated from domain events.
/// </summary>
public class EventMetric : BaseEntity
{
    public string EventType { get; set; } = string.Empty; // AppointmentScheduled, PatientCreated, etc.
    public DateTime OccurredAt { get; set; }
    public Guid AggregateId { get; set; }
    public Dictionary<string, string> Properties { get; set; } = new();
}
