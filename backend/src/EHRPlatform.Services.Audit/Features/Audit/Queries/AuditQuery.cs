using EHRPlatform.Common.CQRS;

namespace EHRPlatform.Services.Audit.Features.Audit.Queries;

/// <summary>
/// Get audit trail for resource.
/// </summary>
public record GetResourceAuditTrailQuery : ICachedQuery<AuditTrailResponseDto>
{
    public string ResourceType { get; init; } = string.Empty;
    public Guid ResourceId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"audit_trail_{ResourceType}_{ResourceId}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 1800; // 30 minutes
}

/// <summary>
/// Get user audit activity.
/// </summary>
public record GetUserAuditActivityQuery : ICachedQuery<UserAuditActivityDto>
{
    public Guid UserId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"audit_user_{UserId}_{FromDate:yyyyMMdd}_{ToDate:yyyyMMdd}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 1800;
}

/// <summary>
/// Get compliance reports.
/// </summary>
public record GetComplianceReportsQuery : ICachedQuery<List<ComplianceReportDto>>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }

    public string CacheKey => $"compliance_reports_{FromDate:yyyyMMdd}_{ToDate:yyyyMMdd}";
    public int CacheDurationSeconds => 3600;
}

/// <summary>
/// Audit trail response DTO.
/// </summary>
public class AuditTrailResponseDto
{
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public List<AuditEntryDto> Entries { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class AuditEntryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? PiiIndicators { get; set; }
    public int AccessLevel { get; set; }
    public string? ChangeDetails { get; set; }
    public string? FailureReason { get; set; }
}

/// <summary>
/// User audit activity DTO.
/// </summary>
public class UserAuditActivityDto
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public List<ActivitySummaryDto> Activities { get; set; } = new();
    public int TotalActions { get; set; }
    public int FailedActions { get; set; }
}

public class ActivitySummaryDto
{
    public string Action { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime LastOccurred { get; set; }
}

/// <summary>
/// Compliance report DTO.
/// </summary>
public class ComplianceReportDto
{
    public Guid Id { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalActions { get; set; }
    public int FailedActions { get; set; }
    public int DataAccess { get; set; }
    public int DataChanges { get; set; }
    public int UnauthorizedAttempts { get; set; }
    public List<string> PiiAccessed { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}
