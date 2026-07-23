using EHRPlatform.Common.Entities;
using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Billing.Domain;

/// <summary>
/// Invoice aggregate root.
/// Manages billing, charges, insurance claims, and payments for healthcare services.
/// </summary>
public class Invoice : AuditableEntity
{
    /// <summary>
    /// Gets or sets the patient identifier.
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// Gets or sets the appointment identifier (optional).
    /// Links the invoice to a specific appointment if applicable.
    /// </summary>
    public Guid? AppointmentId { get; set; }

    /// <summary>
    /// Gets or sets the unique invoice number.
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service date.
    /// </summary>
    public DateTime ServiceDate { get; set; }

    /// <summary>
    /// Gets or sets the payment due date.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the invoice status.
    /// Possible values: Draft, Submitted, Pending, Paid, PartiallyPaid, Overdue, Cancelled
    /// </summary>
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Gets or sets the subtotal (before tax and insurance calculations).
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Gets or sets the calculated tax amount.
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the amount the insurance is responsible for.
    /// </summary>
    public decimal InsuranceResponsibility { get; set; }

    /// <summary>
    /// Gets or sets the amount the patient is responsible for.
    /// </summary>
    public decimal PatientResponsibility { get; set; }

    /// <summary>
    /// Gets or sets the total invoice amount (SubTotal + Tax).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the total amount paid so far.
    /// </summary>
    public decimal AmountPaid { get; set; }

    /// <summary>
    /// Gets the remaining balance due.
    /// </summary>
    public decimal BalanceDue => TotalAmount - AmountPaid;

    /// <summary>
    /// Gets or sets the insurance provider name.
    /// </summary>
    public string? InsuranceProvider { get; set; }

    /// <summary>
    /// Gets or sets the insurance policy number.
    /// </summary>
    public string? InsurancePolicyNumber { get; set; }

    /// <summary>
    /// Gets or sets optional notes about the invoice.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets the collection of line items on this invoice.
    /// </summary>
    public ICollection<LineItem> LineItems { get; } = new List<LineItem>();

    /// <summary>
    /// Gets the collection of payments received on this invoice.
    /// </summary>
    public ICollection<Payment> Payments { get; } = new List<Payment>();

    /// <summary>
    /// Gets the collection of insurance claims associated with this invoice.
    /// </summary>
    public ICollection<InsuranceClaim> InsuranceClaims { get; } = new List<InsuranceClaim>();

    private readonly List<IntegrationEvent> _domainEvents = new();

    /// <summary>
    /// Adds a line item to the invoice.
    /// </summary>
    /// <param name="description">Description of the service or charge.</param>
    /// <param name="cptCode">Current Procedural Terminology code.</param>
    /// <param name="quantity">Quantity of services.</param>
    /// <param name="unitPrice">Price per unit.</param>
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

    /// <summary>
    /// Calculates the total amounts including subtotal, tax, and total.
    /// </summary>
    public void CalculateTotals()
    {
        SubTotal = LineItems.Sum(l => l.Amount);
        TaxAmount = SubTotal * 0.08m; // 8% tax (configurable)
        TotalAmount = SubTotal + TaxAmount;
    }

    /// <summary>
    /// Records a payment received on this invoice.
    /// </summary>
    /// <param name="amount">Payment amount.</param>
    /// <param name="method">Payment method (Credit Card, Check, ACH, Insurance).</param>
    /// <param name="reference">Payment reference (Transaction ID, Check #, etc.).</param>
    /// <exception cref="InvalidOperationException">Thrown if payment amount is invalid or exceeds total.</exception>
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
            Method = method,
            Reference = reference,
            ReceivedAt = DateTime.UtcNow
        };
        Payments.Add(payment);

        AmountPaid += amount;

        var newStatus = AmountPaid >= TotalAmount ? "Paid" : "PartiallyPaid";
        RaiseEvent(new PaymentReceivedEvent(Id, PatientId, amount, newStatus));
    }

    /// <summary>
    /// Submits the invoice to insurance.
    /// </summary>
    /// <param name="provider">Insurance provider name.</param>
    /// <param name="policyNumber">Insurance policy number.</param>
    /// <exception cref="InvalidOperationException">Thrown if invoice status is not Draft.</exception>
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

    /// <summary>
    /// Marks the invoice as fully paid.
    /// </summary>
    public void MarkPaid()
    {
        if (Status == "Paid")
            return;

        Status = "Paid";
        RaiseEvent(new InvoicePaidEvent(Id, PatientId, TotalAmount));
    }

    /// <summary>
    /// Cancels the invoice.
    /// </summary>
    /// <param name="reason">Reason for cancellation.</param>
    /// <exception cref="InvalidOperationException">Thrown if invoice is already paid.</exception>
    public void Cancel(string reason = "")
    {
        if (Status == "Paid")
            throw new InvalidOperationException("Cannot cancel paid invoice");

        Status = "Cancelled";
        RaiseEvent(new InvoiceCancelledEvent(Id, PatientId, reason));
    }

    /// <summary>
    /// Generates a unique claim number.
    /// Format: CLM-YYYYMMDD-XXXXXX
    /// </summary>
    /// <returns>Generated claim number.</returns>
    private string GenerateClaimNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(100000, 999999);
        return $"CLM-{timestamp}-{random}";
    }

    /// <summary>
    /// Raises a domain event.
    /// </summary>
    /// <param name="event">The domain event to raise.</param>
    public void RaiseEvent(IntegrationEvent @event) => _domainEvents.Add(@event);

    /// <summary>
    /// Gets all raised domain events.
    /// </summary>
    /// <returns>Read-only list of domain events.</returns>
    public IReadOnlyList<IntegrationEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    /// <summary>
    /// Clears all raised domain events.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
