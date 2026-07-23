namespace EHRPlatform.Services.Billing.Application.Claims.Responses;

/// <summary>
/// Claim response DTO.
/// Contains insurance claim information.
/// </summary>
public class ClaimResponseDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string InsuranceProvider { get; set; } = string.Empty;
    public string ClaimNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }
}
