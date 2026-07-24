#nullable enable

namespace EHRPlatform.Services.Identity.Application.Identity.DTOs.Responses;

/// <summary>
/// Login response DTO.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT access token for API authentication.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for obtaining new access tokens.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Seconds until access token expires.
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Whether MFA is required to complete login.
    /// </summary>
    public bool MfaRequired { get; set; }

    /// <summary>
    /// Temporary session ID for MFA verification (if MfaRequired is true).
    /// </summary>
    public string? MfaSessionId { get; set; }
}
