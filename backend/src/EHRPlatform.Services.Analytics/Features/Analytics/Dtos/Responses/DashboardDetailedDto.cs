namespace EHRPlatform.Services.Analytics.Features.Analytics.Dtos.Responses;

/// <summary>
/// Dashboard detailed DTO.
/// Single Responsibility: Represent complete dashboard with all widgets and configuration.
/// </summary>
public class DashboardDetailedDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public List<DashboardWidgetDto> Widgets { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
