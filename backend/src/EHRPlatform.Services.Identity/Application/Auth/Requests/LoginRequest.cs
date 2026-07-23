namespace EHRPlatform.Services.Identity.Application.Auth.Requests;

/// <summary>
/// Request DTO for Login.
/// </summary>
public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
