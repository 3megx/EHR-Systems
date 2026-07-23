using AutoMapper;
using EHRPlatform.Services.Analytics.Domain.Entities;
using EHRPlatform.Services.Analytics.Application.Analytics.Responses;

namespace EHRPlatform.Services.Analytics.Application.Analytics.Mappers;

/// <summary>
/// AutoMapper profile for Analytics entities.
/// </summary>
public class AnalyticsMappingProfile : Profile
{
    public AnalyticsMappingProfile()
    {
        CreateMap<Dashboard, DashboardResponse>();
        CreateMap<Report, ReportResponse>();
        CreateMap<AnalyticsMetric, MetricResponse>();
    }
}
