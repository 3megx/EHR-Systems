using AutoMapper;
using EHRPlatform.Services.Analytics.Domain.Entities;
using EHRPlatform.Services.Analytics.Application.Analytics.Responses;

namespace EHRPlatform.Services.Analytics.Application.Analytics.Mappers;

/// <summary>
/// Mapper for Analytics DTOs.
/// </summary>
public class AnalyticsMapper
{
    private readonly IMapper _mapper;

    public AnalyticsMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public DashboardResponse MapToDashboardResponse(Dashboard dashboard)
    {
        return _mapper.Map<DashboardResponse>(dashboard);
    }

    public ReportResponse MapToReportResponse(Report report)
    {
        return _mapper.Map<ReportResponse>(report);
    }
}
