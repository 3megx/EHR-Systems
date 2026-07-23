namespace EHRPlatform.Services.Billing.Features.Reports.Dtos.Responses;

/// <summary>
/// Payment DTO.
/// Records payment information on an invoice.
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
}
