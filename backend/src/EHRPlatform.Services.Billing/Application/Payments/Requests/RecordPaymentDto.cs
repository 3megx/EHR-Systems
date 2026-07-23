namespace EHRPlatform.Services.Billing.Application.Payments.Requests;

/// <summary>
/// Record payment request DTO.
/// Input data for recording a payment.
/// </summary>
public class RecordPaymentDto
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string? Reference { get; set; }
}
