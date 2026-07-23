using EHRPlatform.Common.CQRS;

namespace EHRPlatform.Services.Analytics.Features.Analytics.Queries;

/// <summary>
/// Get metrics for period - CACHED query.
/// </summary>
public record GetMetricsQuery : ICachedQuery<MetricsResponseDto>
{
    public string Category { get; init; } = string.Empty;
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }

    public string CacheKey => $"metrics_{Category}_{PeriodStart:yyyyMMdd}_{PeriodEnd:yyyyMMdd}";
    public int CacheDurationSeconds => 3600; // 1 hour
}

/// <summary>
/// Get KPI summary - CACHED query.
/// </summary>
public record GetKPISummaryQuery : ICachedQuery<KPISummaryDto>
{
    public DateTime? PeriodStart { get; init; }
    public DateTime? PeriodEnd { get; init; }

    public string CacheKey => $"kpi_summary_{PeriodStart?.Date}_{PeriodEnd?.Date}";
    public int CacheDurationSeconds => 3600;
}

/// <summary>
/// Get user dashboard - CACHED query.
/// </summary>
public record GetUserDashboardQuery : ICachedQuery<DashboardResponseDto>
{
    public Guid UserId { get; init; }
    public Guid DashboardId { get; init; }

    public string CacheKey => $"dashboard_{UserId}_{DashboardId}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get user dashboards - CACHED query.
/// </summary>
public record GetUserDashboardsQuery : ICachedQuery<List<DashboardResponseDto>>
{
    public Guid UserId { get; init; }

    public string CacheKey => $"dashboards_user_{UserId}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get report - CACHED query.
/// </summary>
public record GetReportQuery : ICachedQuery<ReportResponseDto>
{
    public Guid ReportId { get; init; }

    public string CacheKey => $"report_{ReportId}";
    public int CacheDurationSeconds => 1800;
}

/// <summary>
/// Get user reports - CACHED query.
/// </summary>
public record GetUserReportsQuery : ICachedQuery<List<ReportResponseDto>>
{
    public Guid UserId { get; init; }

    public string CacheKey => $"reports_user_{UserId}";
    public int CacheDurationSeconds => 1800;
}

/// <summary>
/// Metrics response DTO.
/// </summary>
public class MetricsResponseDto
{
    public string Category { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<MetricItemDto> Metrics { get; set; } = new();
}

public class MetricItemDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal? PreviousPeriodValue { get; set; }
    public decimal? PercentChange { get; set; }
}

/// <summary>
/// KPI summary DTO.
/// </summary>
public class KPISummaryDto
{
    public decimal PatientVolume { get; set; }
    public decimal AppointmentUtilization { get; set; }
    public decimal RevenueTotal { get; set; }
    public decimal AveragePatientSatisfaction { get; set; }
    public int ActiveProviders { get; set; }
    public List<TrendItemDto> Trends { get; set; } = new();
}

public class TrendItemDto
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
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
    public List<decimal> Data { get; set; } = new();
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
    public List<ReportExecutionDto> Executions { get; set; } = new();
}

public class ReportExecutionDto
{
    public Guid Id { get; set; }
    public DateTime ExecutedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? OutputPath { get; set; }
    public int RecordCount { get; set; }
}
