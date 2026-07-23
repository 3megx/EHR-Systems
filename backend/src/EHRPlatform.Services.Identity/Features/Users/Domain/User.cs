using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Identity.Features.Users.Domain;

/// <summary>
/// User entity for identity and access management.
/// HIPAA compliant with audit trail.
/// </summary>
public class User : AuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLogin { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public bool MfaEnabled { get; set; }
    public string? MfaSecret { get; set; }
    public string? MfaSecretBackupCodes { get; set; }

    // Collections
    public ICollection<UserRole> Roles { get; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
    public ICollection<LoginAudit> LoginAudits { get; } = new List<LoginAudit>();

    public bool IsLocked() => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;

    public void Lock() => LockoutEnd = DateTime.UtcNow.AddMinutes(15);

    public void Unlock()
    {
        LockoutEnd = null;
        FailedLoginAttempts = 0;
    }
}

/// <summary>
/// User role assignment.
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

/// <summary>
/// Role with permissions for RBAC.
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<UserRole> Users { get; } = new List<UserRole>();
    public ICollection<RolePermission> Permissions { get; } = new List<RolePermission>();
}

/// <summary>
/// Role permission assignment.
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

/// <summary>
/// Permission for fine-grained access control.
/// Format: "resource:action" (e.g., "patient:read", "patient:write")
/// </summary>
public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Refresh token for JWT renewal.
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public User User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsValid => !IsExpired && !IsRevoked;
}

/// <summary>
/// Login audit trail for security monitoring.
/// </summary>
public class LoginAudit : BaseEntity
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}

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
