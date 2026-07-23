using Mapster;
using EHRPlatform.Common.Mapping;
using EHRPlatform.Services.Analytics.Features.Analytics.Domain;
using EHRPlatform.Services.Analytics.Features.Analytics.Queries;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Analytics.Mappings;

/// <summary>
/// Analytics Mapper
/// Single Responsibility: Convert between Analytics domain models and DTOs.
/// Handles all Analytics-related mappings with optional post-processing.
/// </summary>
public class AnalyticsMapper : MappingServiceBase<AnalyticsMetric, AnalyticsMetricResponseDto>
{
    public AnalyticsMapper(ILogger<AnalyticsMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single analytics metric to response DTO.
    /// </summary>
    public AnalyticsMetricResponseDto MapToResponseDto(AnalyticsMetric metric)
    {
        return MapToDto(metric);
    }

    /// <summary>
    /// Map collection of metrics to paginated DTO.
    /// </summary>
    public AnalyticsMetricListDto MapToListDto(
        ICollection<AnalyticsMetric> metrics,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} analytics metrics to paginated list DTO", metrics.Count);

        return new AnalyticsMetricListDto
        {
            Items = metrics.Adapt<List<AnalyticsMetricResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Map collection of metrics to response DTO list.
    /// </summary>
    public List<AnalyticsMetricResponseDto> MapToResponseDtoList(ICollection<AnalyticsMetric> metrics)
    {
        Logger.LogDebug("Mapping {Count} analytics metrics to response DTO list", metrics.Count);
        return metrics.Adapt<List<AnalyticsMetricResponseDto>>();
    }

    /// <summary>
    /// Map dashboard to detailed DTO with widgets.
    /// </summary>
    public DashboardDetailedDto MapDashboardToDetailedDto(Dashboard dashboard)
    {
        Logger.LogDebug("Mapping dashboard {DashboardId} to detailed DTO", dashboard.Id);

        return new DashboardDetailedDto
        {
            Id = dashboard.Id,
            UserId = dashboard.UserId,
            Name = dashboard.Name,
            Description = dashboard.Description,
            IsDefault = dashboard.IsDefault,
            Widgets = dashboard.DashboardWidgets.Adapt<List<DashboardWidgetDto>>()
                .OrderBy(w => w.Position)
                .ToList(),
            CreatedAt = dashboard.CreatedAt
        };
    }

    /// <summary>
    /// Map dashboard widget to DTO.
    /// </summary>
    public DashboardWidgetDto MapWidgetToDto(DashboardWidget widget)
    {
        Logger.LogDebug("Mapping widget {WidgetId} to DTO", widget.Id);

        return new DashboardWidgetDto
        {
            Id = widget.Id,
            WidgetType = widget.WidgetType,
            Title = widget.Title,
            MetricName = widget.MetricName,
            Position = widget.Position,
            SizeX = widget.SizeX,
            SizeY = widget.SizeY,
            Configuration = widget.Configuration
        };
    }

    /// <summary>
    /// Map report with executions to detailed DTO.
    /// </summary>
    public ReportDetailedDto MapReportToDetailedDto(Report report)
    {
        Logger.LogDebug("Mapping report {ReportId} to detailed DTO", report.Id);

        return new ReportDetailedDto
        {
            Id = report.Id,
            UserId = report.UserId,
            Name = report.Name,
            Description = report.Description,
            ReportType = report.ReportType,
            Metrics = report.Metrics,
            Schedule = report.Schedule,
            LastGeneratedAt = report.LastGeneratedAt,
            IsActive = report.IsActive,
            Executions = report.Executions.Adapt<List<ReportExecutionDto>>()
                .OrderByDescending(e => e.ExecutedAt)
                .ToList(),
            CreatedAt = report.CreatedAt
        };
    }

    /// <summary>
    /// Map report execution to DTO.
    /// </summary>
    public ReportExecutionDto MapExecutionToDto(ReportExecution execution)
    {
        Logger.LogDebug("Mapping report execution {ExecutionId} to DTO", execution.Id);

        return new ReportExecutionDto
        {
            Id = execution.Id,
            ReportId = execution.ReportId,
            ExecutedAt = execution.ExecutedAt,
            Status = execution.Status,
            OutputPath = execution.OutputPath,
            FileSize = execution.FileSize,
            ErrorMessage = execution.ErrorMessage,
            RecordCount = execution.RecordCount
        };
    }
}

/// <summary>
/// Analytics metric response DTO.
/// </summary>
public class AnalyticsMetricResponseDto
{
    public Guid Id { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public Dictionary<string, string> Dimensions { get; set; } = new();
    public string Frequency { get; set; } = string.Empty;
}

/// <summary>
/// Analytics metrics list DTO with pagination.
/// </summary>
public class AnalyticsMetricListDto
{
    public List<AnalyticsMetricResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Dashboard detailed DTO with widgets.
/// </summary>
public class DashboardDetailedDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public List<DashboardWidgetDto> Widgets { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Dashboard widget DTO.
/// </summary>
public class DashboardWidgetDto
{
    public Guid Id { get; set; }
    public string WidgetType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public int Position { get; set; }
    public int SizeX { get; set; }
    public int SizeY { get; set; }
    public Dictionary<string, string> Configuration { get; set; } = new();
}

/// <summary>
/// Report detailed DTO with executions.
/// </summary>
public class ReportDetailedDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public List<string> Metrics { get; set; } = new();
    public string Schedule { get; set; } = string.Empty;
    public DateTime? LastGeneratedAt { get; set; }
    public bool IsActive { get; set; }
    public List<ReportExecutionDto> Executions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Report execution DTO.
/// </summary>
public class ReportExecutionDto
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? OutputPath { get; set; }
    public long? FileSize { get; set; }
    public string? ErrorMessage { get; set; }
    public int RecordCount { get; set; }
}
