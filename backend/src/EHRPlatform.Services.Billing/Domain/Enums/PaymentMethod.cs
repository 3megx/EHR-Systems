namespace EHRPlatform.Services.Billing.Domain.Enums;

/// <summary>
/// Payment method enumeration.
/// Single Responsibility: Define valid payment methods.
/// </summary>
public enum PaymentMethod
{
    CreditCard = 0,
    Check = 1,
    ACH = 2,
    Insurance = 3,
    Cash = 4,
    EFT = 5
}
