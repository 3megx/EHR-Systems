namespace EHRPlatform.Services.Billing.Application.Invoicing.Requests;

/// <summary>
/// Line item request DTO.
/// Represents a single service/charge line to include in an invoice.
/// </summary>
public class LineItemRequestDto
{
    public string Description { get; set; } = string.Empty;
    public string CPTCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
