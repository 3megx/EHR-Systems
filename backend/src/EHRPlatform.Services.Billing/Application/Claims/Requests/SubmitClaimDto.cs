namespace EHRPlatform.Services.Billing.Application.Claims.Requests;

/// <summary>
/// Submit claim request DTO.
/// Input data for submitting an insurance claim.
/// </summary>
public class SubmitClaimDto
{
    public Guid InvoiceId { get; set; }
    public string InsuranceProvider { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
}
