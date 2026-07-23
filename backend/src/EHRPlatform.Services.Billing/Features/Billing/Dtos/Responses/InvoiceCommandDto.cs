namespace EHRPlatform.Services.Billing.Features.Billing.Dtos.Responses;

/// <summary>
/// Invoice command/update DTO.
/// Used for command submissions (not including computed fields).
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
