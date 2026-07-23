namespace EHRPlatform.Services.Billing.Features.Billing.Dtos.Responses;

/// <summary>
/// Invoice response DTO.
/// Contains complete invoice information for API responses.
/// </summary>
public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string? InsuranceProvider { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<LineItemDto> LineItems { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
    public List<InsuranceClaimDto> Claims { get; set; } = new();
}
