namespace EHRPlatform.Common.Security;

/// <summary>
/// Password hashing service for user authentication.
/// Uses PBKDF2-SHA256 with high iteration count.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a plaintext password for secure storage.
    /// Returns Base64-encoded hash with embedded salt.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verify a plaintext password against a stored hash.
    /// Uses constant-time comparison to prevent timing attacks.
    /// </summary>
    bool VerifyPassword(string password, string hash);
}
