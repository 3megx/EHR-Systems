namespace EHRPlatform.Services.Patient.Sagas.Events;

/// <summary>Published by Notification Service when a welcome email/SMS is sent.</summary>
public record WelcomeNotificationSentEvent
{
    public Guid PatientId { get; init; }
    public DateTime SentAt { get; init; } = DateTime.UtcNow;
    public string Channel { get; init; } = "email"; // "email" | "sms"
}

/// <summary>Published by PatientIndexConsumer after ES indexing succeeds.</summary>
public record PatientIndexedEvent
{
    public Guid PatientId { get; init; }
    public string IndexName { get; init; } = "patients";
    public DateTime IndexedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Published when any registration step fails beyond retry limits.</summary>
public record PatientRegistrationFailedEvent
{
    public Guid PatientId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string FailedStep { get; init; } = string.Empty;
    public DateTime FailedAt { get; init; } = DateTime.UtcNow;
}
