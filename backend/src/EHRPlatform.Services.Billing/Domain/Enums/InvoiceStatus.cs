namespace EHRPlatform.Services.Billing.Domain.Enums;

/// <summary>
/// Invoice status enumeration.
/// Single Responsibility: Define valid invoice statuses.
/// </summary>
public enum InvoiceStatus
{
    Draft = 0,
    Submitted = 1,
    Pending = 2,
    Paid = 3,
    PartiallyPaid = 4,
    Overdue = 5,
    Cancelled = 6
}
