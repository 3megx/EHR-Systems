namespace EHRPlatform.Services.Identity.Application.Users.Responses;

/// <summary>
/// Response DTO for User.
/// </summary>
public class UserResponse
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
