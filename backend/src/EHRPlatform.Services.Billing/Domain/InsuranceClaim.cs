using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Billing.Domain;

/// <summary>
/// Insurance claim tracking.
/// Single Responsibility: Track insurance claim for invoice.
/// </summary>
public class InsuranceClaim : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string InsuranceProvider { get; set; } = string.Empty;
    public string ClaimNumber { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? DeniedAt { get; set; }
    public string Status { get; set; } = string.Empty; // Submitted, Approved, Denied, Paid
    public decimal Amount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? DenialReason { get; set; }
    public Invoice Invoice { get; set; } = null!;
}
