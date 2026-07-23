using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Billing.Domain;

/// <summary>
/// Invoice created event.
/// </summary>
public record InvoiceCreatedEvent : IntegrationEvent
{
    public Guid InvoiceId { get; set; }
    public Guid PatientId { get; set; }
    public decimal Amount { get; set; }
    public string InvoiceNumber { get; set; }

    public InvoiceCreatedEvent(Guid id, Guid patientId, decimal amount, string number)
    {
        InvoiceId = id;
        PatientId = patientId;
        Amount = amount;
        InvoiceNumber = number;
    }
}

/// <summary>
/// Invoice submitted to insurance event.
/// </summary>
public record InvoiceSubmittedEvent : IntegrationEvent
{
    public Guid InvoiceId { get; set; }
    public Guid PatientId { get; set; }
    public decimal Amount { get; set; }
    public string InsuranceProvider { get; set; }

    public InvoiceSubmittedEvent(Guid id, Guid patientId, decimal amount, string provider)
    {
        InvoiceId = id;
        PatientId = patientId;
        Amount = amount;
        InsuranceProvider = provider;
    }
}

/// <summary>
/// Payment received event.
/// </summary>
public record PaymentReceivedEvent : IntegrationEvent
{
    public Guid InvoiceId { get; set; }
    public Guid PatientId { get; set; }
    public decimal Amount { get; set; }
    public string NewStatus { get; set; }

    public PaymentReceivedEvent(Guid id, Guid patientId, decimal amount, string status)
    {
        InvoiceId = id;
        PatientId = patientId;
        Amount = amount;
        NewStatus = status;
    }
}

/// <summary>
/// Invoice paid event.
/// </summary>
public record InvoicePaidEvent : IntegrationEvent
{
    public Guid InvoiceId { get; set; }
    public Guid PatientId { get; set; }
    public decimal Amount { get; set; }

    public InvoicePaidEvent(Guid id, Guid patientId, decimal amount)
    {
        InvoiceId = id;
        PatientId = patientId;
        Amount = amount;
    }
}

/// <summary>
/// Invoice cancelled event.
/// </summary>
public record InvoiceCancelledEvent : IntegrationEvent
{
    public Guid InvoiceId { get; set; }
    public Guid PatientId { get; set; }
    public string Reason { get; set; }

    public InvoiceCancelledEvent(Guid id, Guid patientId, string reason)
    {
        InvoiceId = id;
        PatientId = patientId;
        Reason = reason;
    }
}
