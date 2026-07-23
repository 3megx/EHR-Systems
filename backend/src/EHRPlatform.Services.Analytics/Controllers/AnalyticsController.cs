using MediatR;
using Microsoft.AspNetCore.Mvc;
using EHRPlatform.Services.Analytics.Features.Analytics.Commands;
using EHRPlatform.Services.Analytics.Features.Analytics.Queries;

namespace EHRPlatform.Services.Analytics.Controllers;

/// <summary>
/// Analytics and reporting endpoints.
/// KPI dashboards, metrics, custom reports, business intelligence.
/// </summary>
[ApiController]
[Route("api/v1/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get KPI summary (cached).
    /// Executive dashboard with key performance indicators.
    /// </summary>
    [HttpGet("kpi-summary")]
    [ProducesResponseType(typeof(KPISummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKPISummary(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetKPISummaryQuery { PeriodStart = from, PeriodEnd = to },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get metrics by category (cached).
    /// Patients, Appointments, Revenue, Clinical metrics.
    /// </summary>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(MetricsResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMetrics(
        [FromQuery] string category,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetMetricsQuery { Category = category, PeriodStart = from, PeriodEnd = to },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get user dashboards (cached).
    /// List of configured analytics dashboards.
    /// </summary>
    [HttpGet("dashboards")]
    [ProducesResponseType(typeof(List<DashboardResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserDashboards(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetUserDashboardsQuery { UserId = userId },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get dashboard (cached).
    /// Includes widgets and metric data.
    /// </summary>
    [HttpGet("dashboards/{dashboardId}")]
    [ProducesResponseType(typeof(DashboardResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDashboard(
        Guid dashboardId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetUserDashboardQuery { DashboardId = dashboardId, UserId = userId },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create dashboard.
    /// </summary>
    [HttpPost("dashboards")]
    [ProducesResponseType(typeof(DashboardResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDashboard(
        [FromBody] CreateDashboardCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetDashboard), result);
    }

    /// <summary>
    /// Add widget to dashboard.
    /// </summary>
    [HttpPost("dashboards/{dashboardId}/widgets")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddWidget(
        Guid dashboardId,
        [FromBody] AddDashboardWidgetCommand command,
        CancellationToken cancellationToken)
    {
        command = command with { DashboardId = dashboardId };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get user reports (cached).
    /// List of configured reports.
    /// </summary>
    [HttpGet("reports")]
    [ProducesResponseType(typeof(List<ReportResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserReports(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetUserReportsQuery { UserId = userId },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get report (cached).
    /// Includes execution history.
    /// </summary>
    [HttpGet("reports/{reportId}")]
    [ProducesResponseType(typeof(ReportResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReport(
        Guid reportId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetReportQuery { ReportId = reportId },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create report template.
    /// </summary>
    [HttpPost("reports")]
    [ProducesResponseType(typeof(ReportResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateReport(
        [FromBody] CreateReportCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetReport), new { reportId = result.Id }, result);
    }

    /// <summary>
    /// Generate report on-demand.
    /// </summary>
    [HttpPost("reports/{reportId}/generate")]
    [ProducesResponseType(typeof(ReportExecutionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateReport(
        Guid reportId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GenerateReportCommand { ReportId = reportId },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Health check.
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "analytics-service" });
    }
}
