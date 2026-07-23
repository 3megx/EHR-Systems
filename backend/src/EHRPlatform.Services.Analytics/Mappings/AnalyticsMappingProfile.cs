using Mapster;
using EHRPlatform.Services.Analytics.Features.Analytics.Domain;
using EHRPlatform.Services.Analytics.Features.Analytics.Queries;

namespace EHRPlatform.Services.Analytics.Mappings;

/// <summary>
/// Mapster registration profile for Analytics entity mappings.
/// Handles conversion between domain models and DTOs.
/// Single Responsibility: Configure all Analytics-related type mappings.
/// </summary>
public class AnalyticsMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // AnalyticsMetric → AnalyticsMetricResponseDto
        config.NewConfig<AnalyticsMetric, AnalyticsMetricResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.MetricName, src => src.MetricName)
            .Map(dest => dest.Category, src => src.Category)
            .Map(dest => dest.PeriodStart, src => src.PeriodStart)
            .Map(dest => dest.PeriodEnd, src => src.PeriodEnd)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.Unit, src => src.Unit)
            .Map(dest => dest.Dimensions, src => src.Dimensions)
            .Map(dest => dest.Frequency, src => src.Frequency);

        // Dashboard → DashboardDetailedDto
        config.NewConfig<Dashboard, DashboardDetailedDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.IsDefault, src => src.IsDefault)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // DashboardWidget → DashboardWidgetDto
        config.NewConfig<DashboardWidget, DashboardWidgetDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.WidgetType, src => src.WidgetType)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.MetricName, src => src.MetricName)
            .Map(dest => dest.Position, src => src.Position)
            .Map(dest => dest.SizeX, src => src.SizeX)
            .Map(dest => dest.SizeY, src => src.SizeY)
            .Map(dest => dest.Configuration, src => src.Configuration);

        // Report → ReportDetailedDto
        config.NewConfig<Report, ReportDetailedDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.ReportType, src => src.ReportType)
            .Map(dest => dest.Metrics, src => src.Metrics)
            .Map(dest => dest.Schedule, src => src.Schedule)
            .Map(dest => dest.LastGeneratedAt, src => src.LastGeneratedAt)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // ReportExecution → ReportExecutionDto
        config.NewConfig<ReportExecution, ReportExecutionDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ReportId, src => src.ReportId)
            .Map(dest => dest.ExecutedAt, src => src.ExecutedAt)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.OutputPath, src => src.OutputPath)
            .Map(dest => dest.FileSize, src => src.FileSize)
            .Map(dest => dest.ErrorMessage, src => src.ErrorMessage)
            .Map(dest => dest.RecordCount, src => src.RecordCount);

        // AnalyticsMetricResponseDto → AnalyticsMetric (for updates)
        config.NewConfig<AnalyticsMetricResponseDto, AnalyticsMetric>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.MetricName, src => src.MetricName)
            .Map(dest => dest.Category, src => src.Category)
            .Map(dest => dest.PeriodStart, src => src.PeriodStart)
            .Map(dest => dest.PeriodEnd, src => src.PeriodEnd)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.Unit, src => src.Unit)
            .Map(dest => dest.Dimensions, src => src.Dimensions)
            .Map(dest => dest.Frequency, src => src.Frequency);
    }
}
