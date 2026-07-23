namespace EHRPlatform.Services.Analytics.Features.Analytics.Dtos.Responses;

/// <summary>
/// Dashboard widget DTO.
/// Single Responsibility: Represent dashboard widget with data and configuration.
/// </summary>
public class DashboardWidgetDto
{
    public Guid Id { get; set; }
    public string WidgetType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public List<decimal> Data { get; set; } = new();
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
    public List<DashboardWidgetDto> Widgets { get; set; } = new();
}
