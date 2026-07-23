namespace EHRPlatform.Services.Billing.Application.Claims.Responses;

/// <summary>
/// Claim response DTO.
/// Represents an insurance claim with current status and information.
/// </summary>
public class ClaimResponseDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Cancel invoice response DTO.
/// Represents confirmation of invoice cancellation.
/// </summary>
public class CancelInvoiceResponseDto
{
    public Guid InvoiceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CancelledAt { get; set; }
}
