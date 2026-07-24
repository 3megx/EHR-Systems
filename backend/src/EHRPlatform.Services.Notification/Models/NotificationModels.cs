namespace EHRPlatform.Services.Notification.Models;

/// <summary>Notification payload pushed via SignalR to the Angular frontend.</summary>
public record NotificationPayload
{
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? PatientId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string Severity { get; init; } = "info"; // "info" | "warning" | "critical"
}
