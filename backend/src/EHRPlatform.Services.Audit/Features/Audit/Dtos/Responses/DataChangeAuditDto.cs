namespace EHRPlatform.Services.Audit.Features.Audit.Dtos.Responses;

/// <summary>
/// Data change audit DTO.
/// Single Responsibility: Represent detailed data modifications.
/// </summary>
public class DataChangeAuditDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, FieldChangeDto> Changes { get; set; } = new();
}

public class FieldChangeDto
{
    public string FieldName { get; set; } = string.Empty;
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
}
