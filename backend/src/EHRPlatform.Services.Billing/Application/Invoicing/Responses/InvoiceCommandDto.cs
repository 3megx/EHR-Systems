namespace EHRPlatform.Services.Billing.Application.Invoicing.Responses;

/// <summary>
/// Invoice command DTO.
/// Response DTO for create/update invoice operations.
/// </summary>
public class InvoiceCommandDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime ServiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? Notes { get; set; }
}
