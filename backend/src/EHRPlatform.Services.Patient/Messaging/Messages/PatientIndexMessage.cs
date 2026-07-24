namespace EHRPlatform.Services.Patient.Messaging.Messages;

/// <summary>
/// RabbitMQ background job: (re-)index a patient record in Elasticsearch.
/// Triggered after patient create or update.
/// </summary>
public record PatientIndexMessage
{
    public Guid PatientId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string MRN { get; init; } = string.Empty;
    public string Gender { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Status { get; init; } = "Active";
    public string? CorrelationId { get; init; }
}
