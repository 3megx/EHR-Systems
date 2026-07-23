using System.Security.Cryptography;
using System.Text;

namespace EHRPlatform.Common.Security;

/// <summary>
/// PBKDF2-SHA256 password hashing implementation.
/// Designed for user password storage with high security.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSizeBytes = 16; // 128 bits
    private const int HashSizeBytes = 32; // 256 bits (SHA256 output)
    private const int IterationCount = 10000; // NIST recommended minimum

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        try
        {
            var salt = new byte[SaltSizeBytes];

            // Generate cryptographically random salt
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            // Derive password hash using PBKDF2-SHA256
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSizeBytes);

            // Combine salt + hash and return as Base64
            var result = new byte[SaltSizeBytes + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSizeBytes);
            Buffer.BlockCopy(hash, 0, result, SaltSizeBytes, hash.Length);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Password hashing failed", ex);
        }
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            var hashData = Convert.FromBase64String(hash);

            if (hashData.Length < SaltSizeBytes + HashSizeBytes)
                return false;

            // Extract salt from hash
            var salt = new byte[SaltSizeBytes];
            Buffer.BlockCopy(hashData, 0, salt, 0, SaltSizeBytes);

            // Derive hash from provided password using same salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(HashSizeBytes);

            // Extract stored hash
            var storedHash = new byte[HashSizeBytes];
            Buffer.BlockCopy(hashData, SaltSizeBytes, storedHash, 0, HashSizeBytes);

            // Constant-time comparison to prevent timing attacks
            return ConstantTimeEquals(computedHash, storedHash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Constant-time byte array comparison to prevent timing attacks.
    /// This ensures that the comparison time is independent of where the
    /// first difference occurs in the arrays.
    /// </summary>
    private static bool ConstantTimeEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        int result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }
}
