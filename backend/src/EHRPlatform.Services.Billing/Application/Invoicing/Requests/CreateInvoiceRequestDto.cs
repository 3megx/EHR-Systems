namespace EHRPlatform.Services.Billing.Application.Invoicing.Requests;

/// <summary>
/// Create invoice request DTO.
/// Contains all required information to create an invoice with line items.
/// </summary>
public class CreateInvoiceRequestDto
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime ServiceDate { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? Notes { get; set; }
    public List<LineItemRequestDto> LineItems { get; set; } = new();
}

/// <summary>
/// Line item request DTO.
/// Represents a single service/charge on an invoice.
/// </summary>
public class LineItemRequestDto
{
    public string Description { get; set; } = string.Empty;
    public string CPTCode { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}
