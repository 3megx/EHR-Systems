namespace EHRPlatform.Services.Billing.Application.Reports.Responses;

/// <summary>
/// Insurance claim DTO.
/// Represents insurance claim information for an invoice.
/// </summary>
public class InsuranceClaimDto
{
    public Guid Id { get; set; }
    public string InsuranceProvider { get; set; } = string.Empty;
    public string ClaimNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
