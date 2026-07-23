using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Billing.Domain;

/// <summary>
/// Invoice line item (charge/service).
/// Single Responsibility: Represent individual service charge on invoice.
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
