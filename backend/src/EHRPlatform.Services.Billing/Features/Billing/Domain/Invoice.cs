using EHRPlatform.Common.Entities;
using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Billing.Features.Billing.Domain;

/// <summary>
/// Invoice aggregate root.
/// Manages billing, charges, insurance claims, payments.
/// </summary>
public class Invoice : AuditableEntity
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty; // Unique invoice ID
    public DateTime ServiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Pending, Paid, PartiallyPaid, Overdue, Cancelled
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal InsuranceResponsibility { get; set; }
    public decimal PatientResponsibility { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue => TotalAmount - AmountPaid;
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? Notes { get; set; }

    // Collections
    public ICollection<LineItem> LineItems { get; } = new List<LineItem>();
    public ICollection<Payment> Payments { get; } = new List<Payment>();
    public ICollection<InsuranceClaim> InsuranceClaims { get; } = new List<InsuranceClaim>();

    private readonly List<IntegrationEvent> _domainEvents = new();

    public void AddLineItem(string description, string cptCode, decimal quantity, decimal unitPrice)
    {
        var lineItem = new LineItem
        {
            Id = Guid.NewGuid(),
            InvoiceId = Id,
            Description = description,
            CPTCode = cptCode,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Amount = quantity * unitPrice
        };
        LineItems.Add(lineItem);
    }

    public void CalculateTotals()
    {
        SubTotal = LineItems.Sum(l => l.Amount);
        TaxAmount = SubTotal * 0.08m; // 8% tax (configurable)
        TotalAmount = SubTotal + TaxAmount;
    }

    public void RecordPayment(decimal amount, string method, string reference = "")
    {
        if (amount <= 0)
            throw new InvalidOperationException("Payment amount must be positive");

        if (AmountPaid + amount > TotalAmount)
            throw new InvalidOperationException("Payment exceeds invoice total");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            InvoiceId = Id,
            Amount = amount,
            Method = method, // Credit Card, Check, ACH, Insurance
            Reference = reference,
            ReceivedAt = DateTime.UtcNow
        };
        Payments.Add(payment);

        AmountPaid += amount;

        var newStatus = AmountPaid >= TotalAmount ? "Paid" : "PartiallyPaid";
        RaiseEvent(new PaymentReceivedEvent(Id, PatientId, amount, newStatus));
    }

    public void SubmitToInsurance(string provider, string policyNumber)
    {
        if (Status != "Draft")
            throw new InvalidOperationException("Only draft invoices can be submitted");

        InsuranceProvider = provider;
        InsurancePolicyNumber = policyNumber;
        Status = "Submitted";

        var claim = new InsuranceClaim
        {
            Id = Guid.NewGuid(),
            InvoiceId = Id,
            InsuranceProvider = provider,
            ClaimNumber = GenerateClaimNumber(),
            SubmittedAt = DateTime.UtcNow,
            Status = "Submitted",
            Amount = InsuranceResponsibility
        };
        InsuranceClaims.Add(claim);

        RaiseEvent(new InvoiceSubmittedEvent(Id, PatientId, InsuranceResponsibility, provider));
    }

    public void MarkPaid()
    {
        if (Status == "Paid")
            return;

        Status = "Paid";
        RaiseEvent(new InvoicePaidEvent(Id, PatientId, TotalAmount));
    }

    public void Cancel(string reason = "")
    {
        if (Status == "Paid")
            throw new InvalidOperationException("Cannot cancel paid invoice");

        Status = "Cancelled";
        RaiseEvent(new InvoiceCancelledEvent(Id, PatientId, reason));
    }

    private string GenerateClaimNumber()
    {
        // Format: CLM-YYYYMMDD-XXXXXX
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(100000, 999999);
        return $"CLM-{timestamp}-{random}";
    }

    public void RaiseEvent(IntegrationEvent @event) => _domainEvents.Add(@event);
    public IReadOnlyList<IntegrationEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>
/// Invoice line item (charge/service).
/// </summary>
public class LineItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CPTCode { get; set; } = string.Empty; // Current Procedural Terminology
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; } // Quantity * UnitPrice
    public Invoice Invoice { get; set; } = null!;
}

/// <summary>
/// Payment record.
/// </summary>
public class Payment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty; // Credit Card, Check, ACH, Insurance
    public string Reference { get; set; } = string.Empty; // Transaction ID, Check #, etc.
    public DateTime ReceivedAt { get; set; }
    public Invoice Invoice { get; set; } = null!;
}

/// <summary>
/// Insurance claim tracking.
/// </summary>
public class InsuranceClaim : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string InsuranceProvider { get; set; } = string.Empty;
    public string ClaimNumber { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? DeniedAt { get; set; }
    public string Status { get; set; } = string.Empty; // Submitted, Approved, Denied, Paid
    public decimal Amount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? DenialReason { get; set; }
    public Invoice Invoice { get; set; } = null!;
}

/// <summary>
/// Domain events.
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
