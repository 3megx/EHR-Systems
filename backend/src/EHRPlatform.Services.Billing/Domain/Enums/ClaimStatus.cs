namespace EHRPlatform.Services.Billing.Domain.Enums;

/// <summary>
/// Insurance claim status enumeration.
/// Single Responsibility: Define valid claim statuses.
/// </summary>
public enum ClaimStatus
{
    Submitted = 0,
    Approved = 1,
    Denied = 2,
    Paid = 3,
    Appealing = 4,
    OnHold = 5
}
