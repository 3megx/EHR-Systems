using System;
using System.Security.Cryptography;
using System.Text;

namespace EHRPlatform.Common.Security;

/// <summary>
/// PBKDF2-based password hasher.
/// Uses 10,000 iterations and SHA256 for strong password security.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 10000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    /// <summary>
    /// Hash a plaintext password using PBKDF2-SHA256.
    /// </summary>
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password), "Password cannot be empty");

        try
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256);

            var hash = pbkdf2.GetBytes(HashSize);

            // Combine salt + hash
            var result = new byte[SaltSize + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, result, SaltSize, hash.Length);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Password hashing failed", ex);
        }
    }

    /// <summary>
    /// Verify plaintext password against stored hash.
    /// Uses constant-time comparison to prevent timing attacks.
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        try
        {
            var buffer = Convert.FromBase64String(hash);

            if (buffer.Length < SaltSize)
                return false;

            var salt = new byte[SaltSize];
            var storedHash = new byte[buffer.Length - SaltSize];

            Buffer.BlockCopy(buffer, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(buffer, SaltSize, storedHash, 0, buffer.Length - SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256);

            var computedHash = pbkdf2.GetBytes(HashSize);

            // Use constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
        catch
        {
            return false;
        }
    }
}
