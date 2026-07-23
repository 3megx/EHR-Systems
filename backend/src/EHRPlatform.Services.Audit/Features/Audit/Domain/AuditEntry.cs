using EHRPlatform.Common.Entities;

namespace EHRPlatform.Services.Audit.Features.Audit.Domain;

/// <summary>
/// Immutable audit entry (HIPAA-compliant).
/// Cannot be deleted or modified - compliance requirement.
/// </summary>
public class AuditEntry : BaseEntity
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Read, Update, Delete, Export, Print
    public string ResourceType { get; set; } = string.Empty; // Patient, Appointment, Clinical Note, etc.
    public Guid ResourceId { get; set; }
    public string Status { get; set; } = string.Empty; // Success, Failure
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? PiiIndicators { get; set; } // Comma-separated: SSN, DOB, MRN, etc.
    public int AccessLevel { get; set; } // 1=Public, 2=Internal, 3=Confidential, 4=Restricted
    public string? ChangeDetails { get; set; } // JSON: {fieldName: {old, new}}
    public string? FailureReason { get; set; }
    public string IntegrityHash { get; set; } = string.Empty; // SHA-256 for tampering detection
    public int? SessionDurationSeconds { get; set; }
    public bool IsEncrypted { get; set; }

    /// <summary>
    /// Verify data integrity using hash.
    /// </summary>
    public bool VerifyIntegrity(string computedHash) => IntegrityHash == computedHash;
}

/// <summary>
/// Access log (who accessed what and when).
/// </summary>
public class AccessLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public DateTime AccessedAt { get; set; }
    public int DurationSeconds { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public bool IsExport { get; set; }
    public bool IsPrint { get; set; }
}

/// <summary>
/// Data change audit (before/after tracking).
/// </summary>
public class DataChangeAudit : BaseEntity
{
    public Guid UserId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public DateTime ChangedAt { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangeType { get; set; } = string.Empty; // Added, Modified, Deleted
    public string? Reason { get; set; }
}

/// <summary>
/// Compliance report (periodic audit summary).
/// </summary>
public class ComplianceReport : BaseEntity
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalActions { get; set; }
    public int FailedActions { get; set; }
    public int DataAccess { get; set; }
    public int DataChanges { get; set; }
    public int UnauthorizedAttempts { get; set; }
    public List<string> PiiAccessed { get; set; } = new(); // PII types accessed in period
    public string Status { get; set; } = "Generated"; // Generated, Reviewed, Signed, Archived
    public string? SignedBy { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? DigitalSignature { get; set; }
}

/// <summary>
/// Audit log export (immutable snapshot for compliance).
/// </summary>
public class AuditLogExport : BaseEntity
{
    public DateTime ExportedAt { get; set; }
    public Guid ExportedBy { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int RecordCount { get; set; }
    public string FileHash { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string Format { get; set; } = string.Empty; // PDF, CSV, JSON
    public string Status { get; set; } = string.Empty; // Pending, Completed, Failed
    public bool IsEncrypted { get; set; }
}
