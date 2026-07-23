using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Identity.Domain.Entities;

/// <summary>
/// MFA (Multi-Factor Authentication) setup record.
/// </summary>
public class MfaSetup : BaseEntity
{
    public Guid UserId { get; set; }
    public string MfaType { get; set; } = string.Empty; // "TOTP", "SMS", "EMAIL"
    public string Secret { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime SetupAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
}
