namespace EHRPlatform.Services.Billing.Application.Invoicing.Responses;

/// <summary>
/// Outstanding balance DTO.
/// Provides comprehensive balance information for a patient.
/// </summary>
public class OutstandingBalanceDto
{
    public Guid PatientId { get; set; }
    public decimal TotalBalance { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal OverdueAmount { get; set; }
    public List<InvoiceResponseDto> Invoices { get; set; } = new();
}
