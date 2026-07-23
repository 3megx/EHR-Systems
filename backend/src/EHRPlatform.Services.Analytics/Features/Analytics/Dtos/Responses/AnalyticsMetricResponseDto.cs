namespace EHRPlatform.Services.Analytics.Features.Analytics.Dtos.Responses;

/// <summary>
/// Analytics metric response DTO.
/// Single Responsibility: Represent metrics for a period with values and trends.
/// </summary>
public class AnalyticsMetricResponseDto
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
