namespace EHRPlatform.Services.Identity.Application.Auth.Responses;

/// <summary>
/// Response DTO for Login.
/// </summary>
public class LoginResponse
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
