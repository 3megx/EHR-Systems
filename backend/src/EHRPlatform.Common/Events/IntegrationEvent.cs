namespace EHRPlatform.Common.Events;

/// <summary>
/// Base class for integration events published to external systems via Kafka.
/// Used for service-to-service communication and event-driven workflows.
/// </summary>
public abstract class IntegrationEvent
{
    /// <summary>
    /// Unique identifier for this integration event.
    /// </summary>
    public Guid EventId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// When the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Correlation ID for tracing across services.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Causation ID - ID of the command/event that caused this event.
    /// </summary>
    public string? CausationId { get; set; }

    /// <summary>
    /// User ID who triggered this event.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Tenant ID for multi-tenant systems.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Version of this event schema for compatibility.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Source service that published this event.
    /// </summary>
    public string? SourceService { get; set; }

    /// <summary>
    /// Gets the event type name for Kafka topic routing.
    /// </summary>
    public virtual string EventType => GetType().Name;

    /// <summary>
    /// Gets the Kafka topic name for this event.
    /// </summary>
    public virtual string Topic => EventType.ToKebabCase();

    /// <summary>
    /// Gets the event key for Kafka partitioning (usually TenantId or AggregateId).
    /// </summary>
    public virtual string GetPartitionKey()
    {
        return TenantId?.ToString() ?? "default";
    }
}

/// <summary>
/// Outbox event - used to store events reliably before publishing to Kafka.
/// Implements the Outbox Pattern for distributed transactions.
/// </summary>
public class OutboxEvent
{
    /// <summary>
    /// Unique ID for this outbox event.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The integration event data (serialized JSON).
    /// </summary>
    public string? EventData { get; set; }

    /// <summary>
    /// Type of the event for deserialization.
    /// </summary>
    public string? EventType { get; set; }

    /// <summary>
    /// When the event was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the event was published to Kafka.
    /// Null if not yet published.
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Number of attempts to publish this event.
    /// </summary>
    public int PublishAttempts { get; set; }

    /// <summary>
    /// Maximum number of publish attempts before giving up.
    /// </summary>
    public int MaxPublishAttempts { get; set; } = 5;

    /// <summary>
    /// Correlation ID for tracing.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Indicates if the event has been successfully published.
    /// </summary>
    public bool IsPublished => PublishedAt.HasValue;

    /// <summary>
    /// Error message if publishing failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Indicates if this event should be retried.
    /// </summary>
    public bool ShouldRetry => !IsPublished && PublishAttempts < MaxPublishAttempts;
}

/// <summary>
/// Extension methods for event processing.
/// </summary>
public static class IntegrationEventExtensions
{
    /// <summary>
    /// Converts PascalCase to kebab-case.
    /// e.g., "PatientCreatedEvent" -> "patient-created-event"
    /// </summary>
    public static string ToKebabCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
                result.Append('-');

            result.Append(char.ToLowerInvariant(input[i]));
        }

        return result.ToString();
    }
}
