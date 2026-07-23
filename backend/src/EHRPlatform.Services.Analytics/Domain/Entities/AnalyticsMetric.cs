using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Analytics.Domain.Entities;

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
