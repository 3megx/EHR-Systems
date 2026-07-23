using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Audit.Features.Audit.Commands;

/// <summary>
/// Record audit entry command.
/// Called by all services via Kafka or direct API.
/// </summary>
public record RecordAuditEntryCommand : ICommand
{
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string ResourceType { get; init; } = string.Empty;
    public Guid ResourceId { get; init; }
    public bool Success { get; init; } = true;
    public string? FailureReason { get; init; }
    public string IpAddress { get; init; } = string.Empty;
    public string UserAgent { get; init; } = string.Empty;
    public string? PiiIndicators { get; init; }
    public int AccessLevel { get; init; } = 1;
    public string? ChangeDetails { get; init; }
}

public class RecordAuditEntryCommandValidator : AbstractValidator<RecordAuditEntryCommand>
{
    public RecordAuditEntryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.UserEmail).EmailAddress();
        RuleFor(x => x.Action).NotEmpty().Must(a => 
            new[] { "Create", "Read", "Update", "Delete", "Export", "Print" }.Contains(a));
        RuleFor(x => x.ResourceType).NotEmpty();
        RuleFor(x => x.ResourceId).NotEmpty();
        RuleFor(x => x.AccessLevel).GreaterThanOrEqualTo(1).LessThanOrEqualTo(4);
    }
}

/// <summary>
/// Record data change command.
/// </summary>
public record RecordDataChangeCommand : ICommand
{
    public Guid UserId { get; init; }
    public string ResourceType { get; init; } = string.Empty;
    public Guid ResourceId { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public string? Reason { get; init; }
}

/// <summary>
/// Generate compliance report command.
/// </summary>
public record GenerateComplianceReportCommand : ICommand<ComplianceReportResponseDto>
{
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
}

/// <summary>
/// Export audit logs command.
/// </summary>
public record ExportAuditLogsCommand : ICommand<AuditExportResponseDto>
{
    public Guid ExportedBy { get; init; }
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public string Format { get; init; } = "JSON"; // PDF, CSV, JSON
    public bool EncryptFile { get; init; }
}

/// <summary>
/// Compliance report response DTO.
/// </summary>
public class ComplianceReportResponseDto
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
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// Audit export response DTO.
/// </summary>
public class AuditExportResponseDto
{
    public Guid Id { get; set; }
    public DateTime ExportedAt { get; set; }
    public int RecordCount { get; set; }
    public string Format { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; }
    public string? FilePath { get; set; }
    public string Status { get; set; } = string.Empty;
}
