namespace EHRPlatform.Common.Audit;

/// <summary>
/// Immutable audit log record for HIPAA compliance.
/// Every action that affects data is logged here.
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Unique ID for this audit record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant ID for multi-tenant systems.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// User ID who performed the action.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User email (denormalized for reporting).
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// User role at time of action.
    /// </summary>
    public string? UserRole { get; set; }

    /// <summary>
    /// When the action occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The action that was performed (Create, Read, Update, Delete, Export).
    /// </summary>
    public AuditAction Action { get; set; }

    /// <summary>
    /// The type of resource affected.
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the resource affected.
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// Human-readable resource name for easier reporting.
    /// </summary>
    public string? ResourceName { get; set; }

    /// <summary>
    /// The result of the action (Success, Failure, Denied).
    /// </summary>
    public AuditResult Result { get; set; }

    /// <summary>
    /// Details about what changed (JSON format).
    /// Contains before/after values for tracking changes.
    /// </summary>
    public string? Changes { get; set; }

    /// <summary>
    /// Reason for the action (for compliance tracking).
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Error message if the action failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// IP address from which the action was performed.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent / Device information.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Geolocation of the action (if available).
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Correlation ID for linking related audit entries.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Indicates if PII was accessed in this action.
    /// </summary>
    public bool AccessedPII { get; set; }

    /// <summary>
    /// The specific PII fields that were accessed (for tracking consent).
    /// </summary>
    public string? AccessedPIIFields { get; set; }

    /// <summary>
    /// Hash of the audit log for integrity verification (immutability proof).
    /// </summary>
    public string? IntegrityHash { get; set; }

    /// <summary>
    /// Indicates if this record has been verified/sealed.
    /// </summary>
    public bool IsSealed { get; set; }
}

/// <summary>
/// Types of actions that can be audited.
/// </summary>
public enum AuditAction
{
    /// <summary>
    /// Create new resource.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Read/view existing resource.
    /// </summary>
    Read = 2,

    /// <summary>
    /// Update existing resource.
    /// </summary>
    Update = 3,

    /// <summary>
    /// Delete resource (soft or hard).
    /// </summary>
    Delete = 4,

    /// <summary>
    /// Export data outside the system.
    /// </summary>
    Export = 5,

    /// <summary>
    /// Download file or report.
    /// </summary>
    Download = 6,

    /// <summary>
    /// Print document or report.
    /// </summary>
    Print = 7,

    /// <summary>
    /// Send or share data.
    /// </summary>
    Share = 8,

    /// <summary>
    /// Access control action (login, logout, permission change).
    /// </summary>
    AccessControl = 9,

    /// <summary>
    /// Configuration change.
    /// </summary>
    Configure = 10,

    /// <summary>
    /// Administrative action.
    /// </summary>
    Admin = 11,

    /// <summary>
    /// Consent-related action.
    /// </summary>
    Consent = 12
}

/// <summary>
/// Result of an audited action.
/// </summary>
public enum AuditResult
{
    /// <summary>
    /// Action completed successfully.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Action was denied due to insufficient permissions.
    /// </summary>
    Denied = 2,

    /// <summary>
    /// Action failed with an error.
    /// </summary>
    Failure = 3,

    /// <summary>
    /// Action was partially successful.
    /// </summary>
    PartialSuccess = 4,

    /// <summary>
    /// Action generated a warning but succeeded.
    /// </summary>
    Warning = 5
}
