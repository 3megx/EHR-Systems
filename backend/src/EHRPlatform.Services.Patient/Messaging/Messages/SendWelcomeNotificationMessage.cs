namespace EHRPlatform.Services.Patient.Messaging.Messages;

/// <summary>
/// RabbitMQ background job message: send a welcome notification after patient registration.
/// Consumed by the Notification Service (and optionally by a local consumer in this service).
///
/// Transport: RabbitMQ  (background job queue, not Kafka domain event stream)
/// Exchange : ehr.notifications
/// Queue    : ehr.patient.welcome-notification
/// </summary>
public record SendWelcomeNotificationMessage
{
    /// <summary>New patient's ID.</summary>
    public Guid PatientId { get; init; }

    /// <summary>Patient's first name for personalised greeting.</summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>Patient's last name.</summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>Email address to send the welcome message to.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Assigned Medical Record Number.</summary>
    public string MRN { get; init; } = string.Empty;

    /// <summary>Correlation ID from the originating command for tracing.</summary>
    public string? CorrelationId { get; init; }

    /// <summary>Tenant ID for multi-tenant routing.</summary>
    public Guid? TenantId { get; init; }

    /// <summary>When the patient was created (for timestamp in notification).</summary>
    public DateTime RegisteredAt { get; init; } = DateTime.UtcNow;
}
