namespace EHRPlatform.Services.Analytics.Features.Analytics.Dtos.Responses;

/// <summary>
/// KPI summary DTO.
/// Single Responsibility: Represent key performance indicators with trends.
/// </summary>
public class AnalyticsMetricListDto
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
