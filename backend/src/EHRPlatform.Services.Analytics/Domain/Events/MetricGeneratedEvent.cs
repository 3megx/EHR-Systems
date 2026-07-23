using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Analytics.Domain.Events;

public record MetricGeneratedEvent : IntegrationEvent
{
    public Guid MetricId { get; set; }
    public string MetricName { get; set; }
    public string Category { get; set; }
    public decimal Value { get; set; }

    public MetricGeneratedEvent(Guid id, string name, string category, decimal value)
    {
        MetricId = id;
        MetricName = name;
        Category = category;
        Value = value;
    }
}
