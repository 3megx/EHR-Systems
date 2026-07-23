using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Analytics.Features.Analytics.Commands;

/// <summary>
/// Record event metric command.
/// Called by event listeners consuming domain events.
/// </summary>
public record RecordEventMetricCommand : ICommand
{
    public string EventType { get; init; } = string.Empty;
    public Guid AggregateId { get; init; }
    public Dictionary<string, string> Properties { get; init; } = new();
}

/// <summary>
/// Aggregate metrics command.
/// Runs aggregation job for daily/weekly/monthly metrics.
/// </summary>
public record AggregateMetricsCommand : ICommand
{
    public string Frequency { get; init; } = string.Empty; // Daily, Weekly, Monthly
    public DateTime? ForPeriod { get; init; }
}

/// <summary>
/// Create dashboard command.
/// </summary>
public record CreateDashboardCommand : ICommand<DashboardResponseDto>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
}

/// <summary>
/// Add widget to dashboard command.
/// </summary>
public record AddDashboardWidgetCommand : ICommand
{
    public Guid DashboardId { get; init; }
    public string WidgetType { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string MetricName { get; init; } = string.Empty;
}

/// <summary>
/// Create report template command.
/// </summary>
public record CreateReportCommand : ICommand<ReportResponseDto>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ReportType { get; init; } = string.Empty;
    public List<string> Metrics { get; init; } = new();
    public string Schedule { get; init; } = "OnDemand";
}

/// <summary>
/// Generate report command.
/// </summary>
public record GenerateReportCommand : ICommand<ReportExecutionResponseDto>
{
    public Guid ReportId { get; init; }
}

/// <summary>
/// Dashboard response DTO.
/// </summary>
public class DashboardResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public List<WidgetDto> Widgets { get; set; } = new();
}

public class WidgetDto
{
    public Guid Id { get; set; }
    public string WidgetType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public int Position { get; set; }
}

/// <summary>
/// Report response DTO.
/// </summary>
public class ReportResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public List<string> Metrics { get; set; } = new();
    public string Schedule { get; set; } = string.Empty;
    public DateTime? LastGeneratedAt { get; set; }
}

/// <summary>
/// Report execution response DTO.
/// </summary>
public class ReportExecutionResponseDto
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? OutputPath { get; set; }
    public long? FileSize { get; set; }
    public int RecordCount { get; set; }
}
