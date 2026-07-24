using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Analytics.Features.Analytics.Domain;
using Mapster;

namespace EHRPlatform.Services.Analytics.Features.Analytics.Commands;

/// <summary>
/// Record event metric handler.
/// Stores raw event metrics from Kafka events.
/// </summary>
public class RecordEventMetricCommandHandler : ICommandHandler<RecordEventMetricCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RecordEventMetricCommandHandler> _logger;

    public RecordEventMetricCommandHandler(IUnitOfWork unitOfWork, ILogger<RecordEventMetricCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(RecordEventMetricCommand command, CancellationToken cancellationToken)
    {
        var metric = new EventMetric
        {
            Id = Guid.NewGuid(),
            EventType = command.EventType,
            AggregateId = command.AggregateId,
            OccurredAt = DateTime.UtcNow,
            Properties = command.Properties
        };

        var repo = _unitOfWork.Repository<EventMetric>();
        await repo.AddAsync(metric, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Event metric recorded: {EventType}", command.EventType);
    }
}

/// <summary>
/// Aggregate metrics handler.
/// Calculates daily/weekly/monthly aggregates from event metrics.
/// </summary>
public class AggregateMetricsCommandHandler : ICommandHandler<AggregateMetricsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AggregateMetricsCommandHandler> _logger;

    public AggregateMetricsCommandHandler(IUnitOfWork unitOfWork, ILogger<AggregateMetricsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(AggregateMetricsCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Aggregating metrics for {Frequency}", command.Frequency);

        var (periodStart, periodEnd) = GetPeriodDates(command.Frequency, command.ForPeriod ?? DateTime.UtcNow);

        var eventRepo = _unitOfWork.Repository<EventMetric>();
        var events = await eventRepo.ToListAsync(
            q => q.Where(e => e.OccurredAt >= periodStart && e.OccurredAt < periodEnd),
            cancellationToken);

        // Aggregate by event type
        var aggregates = events
            .GroupBy(e => e.EventType)
            .Select(g => new AnalyticsMetric
            {
                Id = Guid.NewGuid(),
                MetricName = $"{g.Key}_count",
                Category = GetCategory(g.Key),
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                Value = g.Count(),
                Unit = "count",
                Frequency = command.Frequency
            })
            .ToList();

        var metricRepo = _unitOfWork.Repository<AnalyticsMetric>();
        foreach (var agg in aggregates)
        {
            await metricRepo.AddAsync(agg, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Aggregated {Count} metrics", aggregates.Count);
    }

    private (DateTime, DateTime) GetPeriodDates(string frequency, DateTime date)
    {
        switch (frequency.ToLower())
        {
            case "weekly":
                var startOfWeek = date.AddDays(-(int)date.DayOfWeek);
                var weekStart = startOfWeek.Date;
                return (weekStart, weekStart.AddDays(7));
            case "monthly":
                var monthStart = new DateTime(date.Year, date.Month, 1);
                return (monthStart, monthStart.AddMonths(1));
            default: // daily
                return (date.Date, date.Date.AddDays(1));
        }
    }

    private string GetCategory(string eventType)
    {
        return eventType switch
        {
            "PatientCreated" or "PatientUpdated" => "Patients",
            "AppointmentScheduled" or "AppointmentCompleted" => "Appointments",
            "InvoiceCreated" or "PaymentReceived" => "Revenue",
            "ClinicalNoteCreated" => "Clinical",
            _ => "System"
        };
    }
}

/// <summary>
/// Create dashboard handler.
/// </summary>
public class CreateDashboardCommandHandler : ICommandHandler<CreateDashboardCommand, DashboardResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDashboardCommandHandler> _logger;

    public CreateDashboardCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateDashboardCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DashboardResponseDto> Handle(
        CreateDashboardCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating dashboard for user {UserId}", command.UserId);

        var dashboard = new Dashboard
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Name = command.Name,
            Description = command.Description,
            IsDefault = command.IsDefault
        };

        var repo = _unitOfWork.Repository<Dashboard>();
        await repo.AddAsync(dashboard, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return dashboard.Adapt<DashboardResponseDto>();
    }
}

/// <summary>
/// Create report handler.
/// </summary>
public class CreateReportCommandHandler : ICommandHandler<CreateReportCommand, ReportResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateReportCommandHandler> _logger;

    public CreateReportCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateReportCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ReportResponseDto> Handle(
        CreateReportCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating report template {Name}", command.Name);

        var report = new Report
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Name = command.Name,
            Description = command.Description,
            ReportType = command.ReportType,
            Metrics = command.Metrics,
            Schedule = command.Schedule
        };

        var repo = _unitOfWork.Repository<Report>();
        await repo.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return report.Adapt<ReportResponseDto>();
    }
}

/// <summary>
/// Generate report handler.
/// </summary>
public class GenerateReportCommandHandler : ICommandHandler<GenerateReportCommand, ReportExecutionResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateReportCommandHandler> _logger;

    public GenerateReportCommandHandler(IUnitOfWork unitOfWork, ILogger<GenerateReportCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ReportExecutionResponseDto> Handle(
        GenerateReportCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating report {ReportId}", command.ReportId);

        var reportRepo = _unitOfWork.Repository<Report>();
        var report = await reportRepo.FirstOrDefaultAsync(
            q => q.Where(r => r.Id == command.ReportId),
            cancellationToken);

        if (report == null)
            throw new InvalidOperationException($"Report {command.ReportId} not found");

        // Create execution record
        var execution = new ReportExecution
        {
            Id = Guid.NewGuid(),
            ReportId = report.Id,
            ExecutedAt = DateTime.UtcNow,
            Status = "Completed",
            RecordCount = 0
        };

        var execRepo = _unitOfWork.Repository<ReportExecution>();
        await execRepo.AddAsync(execution, cancellationToken);

        report.LastGeneratedAt = DateTime.UtcNow;
        await reportRepo.UpdateAsync(report, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Report generated {ExecutionId}", execution.Id);

        return execution.Adapt<ReportExecutionResponseDto>();
    }
}
