using Mapster;
using EHRPlatform.Common.Mapping;
using EHRPlatform.Services.Analytics.Domain.Entities;
using EHRPlatform.Services.Analytics.Application.Analytics.Responses;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Analytics.Application.Analytics.Mappers;

/// <summary>
/// Analytics Mapper
/// Single Responsibility: Convert between Analytics domain models and DTOs.
/// </summary>
public class AnalyticsMapper : MappingServiceBase<Dashboard, DashboardResponse>
{
    public AnalyticsMapper(ILogger<AnalyticsMapper> logger) : base(logger)
    {
    }

    public DashboardListDto MapToDashboardListDto(
        ICollection<Dashboard> dashboards,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} dashboards to paginated list DTO", dashboards.Count);

        return new DashboardListDto
        {
            Items = dashboards.Adapt<List<DashboardResponse>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public ReportListDto MapToReportListDto(
        ICollection<Report> reports,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} reports to paginated list DTO", reports.Count);

        return new ReportListDto
        {
            Items = reports.Adapt<List<ReportResponse>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
