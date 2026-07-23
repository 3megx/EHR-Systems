namespace EHRPlatform.Services.Billing.Application.Invoicing.Responses;

/// <summary>
/// Line item DTO.
/// Represents a service or item on an invoice.
/// </summary>
public class LineItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CPTCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
}
