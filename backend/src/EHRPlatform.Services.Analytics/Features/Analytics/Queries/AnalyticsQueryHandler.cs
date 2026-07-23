using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Analytics.Features.Analytics.Domain;
using EHRPlatform.Services.Analytics.Features.Analytics.Dtos.Responses;
using Mapster;

namespace EHRPlatform.Services.Analytics.Features.Analytics.Queries;

/// <summary>
/// Get metrics handler.
/// </summary>
public class GetMetricsQueryHandler : IQueryHandler<GetMetricsQuery, AnalyticsMetricResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetMetricsQueryHandler> _logger;

    public GetMetricsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetMetricsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AnalyticsMetricResponseDto> Handle(GetMetricsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching metrics for {Category}", request.Category);

        var repo = _unitOfWork.Repository<AnalyticsMetric>();
        var metrics = await repo.ToListAsync(
            q => q.Where(m =>
                m.Category == request.Category &&
                m.PeriodStart >= request.PeriodStart &&
                m.PeriodEnd <= request.PeriodEnd),
            cancellationToken);

        return new AnalyticsMetricResponseDto
        {
            Category = request.Category,
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            Metrics = metrics.Select(m => new MetricItemDto
            {
                Name = m.MetricName,
                Value = m.Value,
                Unit = m.Unit
            }).ToList()
        };
    }
}

/// <summary>
/// Get KPI summary handler.
/// </summary>
public class GetKPISummaryQueryHandler : IQueryHandler<GetKPISummaryQuery, AnalyticsMetricListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetKPISummaryQueryHandler> _logger;

    public GetKPISummaryQueryHandler(IUnitOfWork unitOfWork, ILogger<GetKPISummaryQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AnalyticsMetricListDto> Handle(GetKPISummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating KPI summary");

        var periodStart = request.PeriodStart ?? DateTime.UtcNow.AddDays(-30);
        var periodEnd = request.PeriodEnd ?? DateTime.UtcNow;

        var repo = _unitOfWork.Repository<AnalyticsMetric>();
        var metrics = await repo.ToListAsync(
            q => q.Where(m => m.PeriodStart >= periodStart && m.PeriodEnd <= periodEnd),
            cancellationToken);

        var summary = new AnalyticsMetricListDto
        {
            PatientVolume = metrics.Where(m => m.MetricName.Contains("patient")).Sum(m => m.Value),
            AppointmentUtilization = metrics.Where(m => m.MetricName.Contains("appointment")).Sum(m => m.Value) / 100m,
            RevenueTotal = metrics.Where(m => m.Unit == "USD").Sum(m => m.Value),
            Trends = new List<TrendItemDto>()
        };

        return summary;
    }
}

/// <summary>
/// Get user dashboard handler.
/// </summary>
public class GetUserDashboardQueryHandler : IQueryHandler<GetUserDashboardQuery, DashboardResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserDashboardQueryHandler> _logger;

    public GetUserDashboardQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUserDashboardQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DashboardResponseDto> Handle(GetUserDashboardQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching dashboard {DashboardId} for user {UserId}",
            request.DashboardId, request.UserId);

        var repo = _unitOfWork.Repository<Dashboard>();
        var dashboard = await repo.FirstOrDefaultAsync(
            q => q.Where(d => d.Id == request.DashboardId && d.UserId == request.UserId),
            cancellationToken);

        if (dashboard == null)
            throw new InvalidOperationException($"Dashboard {request.DashboardId} not found");

        var dto = dashboard.Adapt<DashboardResponseDto>();
        dto.Widgets = dashboard.DashboardWidgets.Select(w => new DashboardWidgetDto
        {
            Id = w.Id,
            WidgetType = w.WidgetType,
            Title = w.Title,
            MetricName = w.MetricName
        }).ToList();

        return dto;
    }
}

/// <summary>
/// Get user dashboards handler.
/// </summary>
public class GetUserDashboardsQueryHandler : IQueryHandler<GetUserDashboardsQuery, List<DashboardResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserDashboardsQueryHandler> _logger;

    public GetUserDashboardsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUserDashboardsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<DashboardResponseDto>> Handle(GetUserDashboardsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching dashboards for user {UserId}", request.UserId);

        var repo = _unitOfWork.Repository<Dashboard>();
        var dashboards = await repo.ToListAsync(
            q => q.Where(d => d.UserId == request.UserId),
            cancellationToken);

        return dashboards.Select(d => new DashboardResponseDto
        {
            Id = d.Id,
            UserId = d.UserId,
            Name = d.Name,
            Description = d.Description,
            IsDefault = d.IsDefault,
            Widgets = d.DashboardWidgets.Select(w => new DashboardWidgetDto
            {
                Id = w.Id,
                WidgetType = w.WidgetType,
                Title = w.Title,
                MetricName = w.MetricName
            }).ToList()
        }).ToList();
    }
}

/// <summary>
/// Get report handler.
/// </summary>
public class GetReportQueryHandler : IQueryHandler<GetReportQuery, ReportResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetReportQueryHandler> _logger;

    public GetReportQueryHandler(IUnitOfWork unitOfWork, ILogger<GetReportQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ReportResponseDto> Handle(GetReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching report {ReportId}", request.ReportId);

        var repo = _unitOfWork.Repository<Report>();
        var report = await repo.FirstOrDefaultAsync(
            q => q.Where(r => r.Id == request.ReportId),
            cancellationToken);

        if (report == null)
            throw new InvalidOperationException($"Report {request.ReportId} not found");

        return report.Adapt<ReportResponseDto>();
    }
}

/// <summary>
/// Get user reports handler.
/// </summary>
public class GetUserReportsQueryHandler : IQueryHandler<GetUserReportsQuery, List<ReportResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserReportsQueryHandler> _logger;

    public GetUserReportsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUserReportsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<ReportResponseDto>> Handle(GetUserReportsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching reports for user {UserId}", request.UserId);

        var repo = _unitOfWork.Repository<Report>();
        var reports = await repo.ToListAsync(
            q => q.Where(r => r.UserId == request.UserId),
            cancellationToken);

        return reports.Adapt<List<ReportResponseDto>>();
    }
}
