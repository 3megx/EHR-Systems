namespace EHRPlatform.Common.Security;

/// <summary>
/// Service for securely hashing and verifying passwords.
/// Implements PBKDF2 with SHA256 for password security.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a plaintext password.
    /// </summary>
    /// <param name="password">Password to hash</param>
    /// <returns>Hashed password (Base64 encoded)</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verify plaintext password against hash.
    /// </summary>
    /// <param name="password">Password to verify</param>
    /// <param name="hash">Hash to verify against</param>
    /// <returns>True if password matches hash</returns>
    bool VerifyPassword(string password, string hash);
}
