namespace EHRPlatform.Services.Billing.Application.Payments.Responses;

/// <summary>
/// Payment response DTO.
/// Contains payment information.
/// </summary>
public class PaymentResponseDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
