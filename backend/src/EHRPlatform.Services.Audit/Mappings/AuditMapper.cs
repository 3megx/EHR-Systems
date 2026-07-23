using Mapster;
using EHRPlatform.Common.Mapping;
using EHRPlatform.Services.Audit.Features.Audit.Domain;
using EHRPlatform.Services.Audit.Features.Audit.Queries;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Audit.Mappings;

/// <summary>
/// Audit Mapper
/// Single Responsibility: Convert between Audit domain models and DTOs.
/// Handles all Audit-related mappings with optional post-processing.
/// </summary>
public class AuditMapper : MappingServiceBase<AuditEntry, AuditEntryResponseDto>
{
    public AuditMapper(ILogger<AuditMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single audit entry to response DTO.
    /// </summary>
    public AuditEntryResponseDto MapToResponseDto(AuditEntry entry)
    {
        return MapToDto(entry);
    }

    /// <summary>
    /// Map audit entry to detailed DTO.
    /// </summary>
    public AuditEntryDetailedDto MapToDetailedDto(AuditEntry entry)
    {
        Logger.LogDebug("Mapping audit entry {AuditId} to detailed DTO", entry.Id);

        return new AuditEntryDetailedDto
        {
            Id = entry.Id,
            UserId = entry.UserId,
            UserEmail = entry.UserEmail,
            Action = entry.Action,
            ResourceType = entry.ResourceType,
            ResourceId = entry.ResourceId,
            Status = entry.Status,
            Timestamp = entry.Timestamp,
            IpAddress = entry.IpAddress,
            UserAgent = entry.UserAgent,
            PiiIndicators = entry.PiiIndicators,
            AccessLevel = entry.AccessLevel,
            ChangeDetails = entry.ChangeDetails,
            FailureReason = entry.FailureReason,
            SessionDurationSeconds = entry.SessionDurationSeconds,
            IsEncrypted = entry.IsEncrypted,
            CreatedAt = entry.CreatedAt
        };
    }

    /// <summary>
    /// Map collection of audit entries to paginated DTO.
    /// </summary>
    public AuditEntryListDto MapToListDto(
        ICollection<AuditEntry> entries,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} audit entries to paginated list DTO", entries.Count);

        return new AuditEntryListDto
        {
            Items = entries.Adapt<List<AuditEntryResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Map collection of audit entries to response DTO list.
    /// </summary>
    public List<AuditEntryResponseDto> MapToResponseDtoList(ICollection<AuditEntry> entries)
    {
        Logger.LogDebug("Mapping {Count} audit entries to response DTO list", entries.Count);
        return entries.Adapt<List<AuditEntryResponseDto>>();
    }

    /// <summary>
    /// Map access log to DTO.
    /// </summary>
    public AccessLogDto MapAccessLogToDto(AccessLog log)
    {
        Logger.LogDebug("Mapping access log {AccessLogId} to DTO", log.Id);

        return new AccessLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            UserEmail = log.UserEmail,
            ResourceType = log.ResourceType,
            ResourceId = log.ResourceId,
            AccessedAt = log.AccessedAt,
            DurationSeconds = log.DurationSeconds,
            IpAddress = log.IpAddress,
            IsExport = log.IsExport,
            IsPrint = log.IsPrint
        };
    }

    /// <summary>
    /// Map data change audit to DTO.
    /// </summary>
    public DataChangeAuditDto MapDataChangeToDto(DataChangeAudit change)
    {
        Logger.LogDebug("Mapping data change audit {ChangeId} to DTO", change.Id);

        return new DataChangeAuditDto
        {
            Id = change.Id,
            UserId = change.UserId,
            ResourceType = change.ResourceType,
            ResourceId = change.ResourceId,
            ChangedAt = change.ChangedAt,
            FieldName = change.FieldName,
            OldValue = change.OldValue,
            NewValue = change.NewValue,
            ChangeType = change.ChangeType,
            Reason = change.Reason
        };
    }

    /// <summary>
    /// Map compliance report to DTO.
    /// </summary>
    public ComplianceReportDto MapComplianceReportToDto(ComplianceReport report)
    {
        Logger.LogDebug("Mapping compliance report {ReportId} to DTO", report.Id);

        return new ComplianceReportDto
        {
            Id = report.Id,
            PeriodStart = report.PeriodStart,
            PeriodEnd = report.PeriodEnd,
            TotalActions = report.TotalActions,
            FailedActions = report.FailedActions,
            DataAccess = report.DataAccess,
            DataChanges = report.DataChanges,
            UnauthorizedAttempts = report.UnauthorizedAttempts,
            PiiAccessed = report.PiiAccessed,
            Status = report.Status,
            SignedBy = report.SignedBy,
            SignedAt = report.SignedAt
        };
    }
}

/// <summary>
/// Audit entry detailed DTO.
/// </summary>
public class AuditEntryDetailedDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? PiiIndicators { get; set; }
    public int AccessLevel { get; set; }
    public string? ChangeDetails { get; set; }
    public string? FailureReason { get; set; }
    public int? SessionDurationSeconds { get; set; }
    public bool IsEncrypted { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Audit entry list DTO with pagination.
/// </summary>
public class AuditEntryListDto
{
    public List<AuditEntryResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Access log DTO.
/// </summary>
public class AccessLogDto
{
    public Guid Id { get; set; }
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
/// Data change audit DTO.
/// </summary>
public class DataChangeAuditDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public DateTime ChangedAt { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? Reason { get; set; }
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
    public string? SignedBy { get; set; }
    public DateTime? SignedAt { get; set; }
}
