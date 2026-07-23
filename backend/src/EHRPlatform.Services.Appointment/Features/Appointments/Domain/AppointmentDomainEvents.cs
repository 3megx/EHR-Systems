using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Domain;

/// <summary>
/// Domain event raised when an appointment is scheduled.
/// </summary>
public record AppointmentScheduledEvent : IntegrationEvent
{
    /// <summary>
    /// Gets the appointment identifier.
    /// </summary>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Gets the patient identifier.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Gets the scheduled start time.
    /// </summary>
    public DateTime ScheduledStart { get; set; }

    /// <summary>
    /// Gets the appointment type.
    /// </summary>
    public string AppointmentType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppointmentScheduledEvent"/> class.
    /// </summary>
    /// <param name="id">The appointment identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <param name="providerId">The provider identifier.</param>
    /// <param name="start">The scheduled start time.</param>
    /// <param name="type">The appointment type.</param>
    public AppointmentScheduledEvent(Guid id, Guid patientId, Guid providerId, DateTime start, string type)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        ScheduledStart = start;
        AppointmentType = type;
    }
}

/// <summary>
/// Domain event raised when an appointment is confirmed.
/// </summary>
public record AppointmentConfirmedEvent : IntegrationEvent
{
    /// <summary>
    /// Gets the appointment identifier.
    /// </summary>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Gets the patient identifier.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Gets the scheduled start time.
    /// </summary>
    public DateTime ScheduledStart { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppointmentConfirmedEvent"/> class.
    /// </summary>
    /// <param name="id">The appointment identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <param name="providerId">The provider identifier.</param>
    /// <param name="start">The scheduled start time.</param>
    public AppointmentConfirmedEvent(Guid id, Guid patientId, Guid providerId, DateTime start)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        ScheduledStart = start;
    }
}

/// <summary>
/// Domain event raised when an appointment is cancelled.
/// </summary>
public record AppointmentCancelledEvent : IntegrationEvent
{
    /// <summary>
    /// Gets the appointment identifier.
    /// </summary>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Gets the patient identifier.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Gets the cancellation reason.
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppointmentCancelledEvent"/> class.
    /// </summary>
    /// <param name="id">The appointment identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <param name="providerId">The provider identifier.</param>
    /// <param name="reason">The cancellation reason.</param>
    public AppointmentCancelledEvent(Guid id, Guid patientId, Guid providerId, string reason)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        Reason = reason;
    }
}

/// <summary>
/// Domain event raised when an appointment is checked in.
/// </summary>
public record AppointmentCheckedInEvent : IntegrationEvent
{
    /// <summary>
    /// Gets the appointment identifier.
    /// </summary>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Gets the patient identifier.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Gets the check-in time.
    /// </summary>
    public DateTime CheckInTime { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppointmentCheckedInEvent"/> class.
    /// </summary>
    /// <param name="id">The appointment identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <param name="providerId">The provider identifier.</param>
    /// <param name="checkIn">The check-in time.</param>
    public AppointmentCheckedInEvent(Guid id, Guid patientId, Guid providerId, DateTime checkIn)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        CheckInTime = checkIn;
    }
}

/// <summary>
/// Domain event raised when an appointment is completed.
/// </summary>
public record AppointmentCompletedEvent : IntegrationEvent
{
    /// <summary>
    /// Gets the appointment identifier.
    /// </summary>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Gets the patient identifier.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public Guid ProviderId { get; set; }

    /// <summary>
    /// Gets the completion time.
    /// </summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppointmentCompletedEvent"/> class.
    /// </summary>
    /// <param name="id">The appointment identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <param name="providerId">The provider identifier.</param>
    /// <param name="completed">The completion time.</param>
    public AppointmentCompletedEvent(Guid id, Guid patientId, Guid providerId, DateTime completed)
    {
        AppointmentId = id;
        PatientId = patientId;
        ProviderId = providerId;
        CompletedAt = completed;
    }
}
