using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Analytics.Domain.Entities;

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
