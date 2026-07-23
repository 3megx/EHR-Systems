using Mapster;
using EHRPlatform.Services.Audit.Features.Audit.Domain;
using EHRPlatform.Services.Audit.Features.Audit.Queries;

namespace EHRPlatform.Services.Audit.Mappings;

/// <summary>
/// Mapster registration profile for Audit entity mappings.
/// Handles conversion between domain models and DTOs.
/// Single Responsibility: Configure all Audit-related type mappings.
/// </summary>
public class AuditMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // AuditEntry → AuditEntryResponseDto
        config.NewConfig<AuditEntry, AuditEntryResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.UserEmail, src => src.UserEmail)
            .Map(dest => dest.Action, src => src.Action)
            .Map(dest => dest.ResourceType, src => src.ResourceType)
            .Map(dest => dest.ResourceId, src => src.ResourceId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Timestamp, src => src.Timestamp)
            .Map(dest => dest.IpAddress, src => src.IpAddress)
            .Map(dest => dest.AccessLevel, src => src.AccessLevel);

        // AccessLog → AccessLogDto
        config.NewConfig<AccessLog, AccessLogDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.UserEmail, src => src.UserEmail)
            .Map(dest => dest.ResourceType, src => src.ResourceType)
            .Map(dest => dest.ResourceId, src => src.ResourceId)
            .Map(dest => dest.AccessedAt, src => src.AccessedAt)
            .Map(dest => dest.DurationSeconds, src => src.DurationSeconds)
            .Map(dest => dest.IpAddress, src => src.IpAddress)
            .Map(dest => dest.IsExport, src => src.IsExport)
            .Map(dest => dest.IsPrint, src => src.IsPrint);

        // DataChangeAudit → DataChangeAuditDto
        config.NewConfig<DataChangeAudit, DataChangeAuditDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.ResourceType, src => src.ResourceType)
            .Map(dest => dest.ResourceId, src => src.ResourceId)
            .Map(dest => dest.ChangedAt, src => src.ChangedAt)
            .Map(dest => dest.FieldName, src => src.FieldName)
            .Map(dest => dest.OldValue, src => src.OldValue)
            .Map(dest => dest.NewValue, src => src.NewValue)
            .Map(dest => dest.ChangeType, src => src.ChangeType)
            .Map(dest => dest.Reason, src => src.Reason);

        // ComplianceReport → ComplianceReportDto
        config.NewConfig<ComplianceReport, ComplianceReportDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PeriodStart, src => src.PeriodStart)
            .Map(dest => dest.PeriodEnd, src => src.PeriodEnd)
            .Map(dest => dest.TotalActions, src => src.TotalActions)
            .Map(dest => dest.FailedActions, src => src.FailedActions)
            .Map(dest => dest.DataAccess, src => src.DataAccess)
            .Map(dest => dest.DataChanges, src => src.DataChanges)
            .Map(dest => dest.UnauthorizedAttempts, src => src.UnauthorizedAttempts)
            .Map(dest => dest.PiiAccessed, src => src.PiiAccessed)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.SignedBy, src => src.SignedBy)
            .Map(dest => dest.SignedAt, src => src.SignedAt);

        // AuditEntryResponseDto → AuditEntry (for updates)
        config.NewConfig<AuditEntryResponseDto, AuditEntry>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.UserEmail, src => src.UserEmail)
            .Map(dest => dest.Action, src => src.Action)
            .Map(dest => dest.ResourceType, src => src.ResourceType)
            .Map(dest => dest.ResourceId, src => src.ResourceId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Timestamp, src => src.Timestamp)
            .Map(dest => dest.IpAddress, src => src.IpAddress);
    }
}

/// <summary>
/// Audit entry response DTO.
/// </summary>
public class AuditEntryResponseDto
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
    public int AccessLevel { get; set; }
}
