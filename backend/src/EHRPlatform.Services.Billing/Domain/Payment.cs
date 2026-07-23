using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Billing.Domain;

/// <summary>
/// Payment record.
/// Single Responsibility: Track payment received for invoice.
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
