using System;
using System.Security.Cryptography;
using System.Text;

namespace EHRPlatform.Common.Security;

/// <summary>
/// AES encryption service for protecting sensitive data.
/// Uses 256-bit AES-GCM for authenticated encryption.
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _encryptionKey;
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public EncryptionService(string encryptionKey)
    {
        if (string.IsNullOrEmpty(encryptionKey))
            throw new ArgumentNullException(nameof(encryptionKey));

        if (encryptionKey.Length < 32)
            throw new ArgumentException(
                "Encryption key must be at least 32 characters (256 bits)",
                nameof(encryptionKey));

        // Use SHA256 of the provided key for consistent 256-bit key
        using var sha = SHA256.Create();
        _encryptionKey = sha.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
    }

    /// <summary>
    /// Encrypt plaintext using AES-256-GCM.
    /// </summary>
    public string Encrypt(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
            throw new ArgumentNullException(nameof(plaintext));

        try
        {
            using var aes = new AesGcm(_encryptionKey);
            var nonce = new byte[NonceSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(nonce);

            var ciphertext = new byte[Encoding.UTF8.GetByteCount(plaintext)];
            var tag = new byte[TagSize];

            aes.Encrypt(
                nonce,
                Encoding.UTF8.GetBytes(plaintext),
                null,
                ciphertext,
                tag);

            // Combine nonce + ciphertext + tag for transmission
            var result = new byte[nonce.Length + ciphertext.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length + ciphertext.Length, tag.Length);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            throw new CryptographicException("Encryption failed", ex);
        }
    }

    /// <summary>
    /// Decrypt ciphertext using AES-256-GCM.
    /// </summary>
    public string Decrypt(string ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext))
            throw new ArgumentNullException(nameof(ciphertext));

        try
        {
            var buffer = Convert.FromBase64String(ciphertext);

            // Extract nonce, ciphertext, and tag
            var nonce = new byte[NonceSize];
            var cypherBytes = new byte[buffer.Length - NonceSize - TagSize];
            var tag = new byte[TagSize];

            Buffer.BlockCopy(buffer, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(buffer, NonceSize, cypherBytes, 0, buffer.Length - NonceSize - TagSize);
            Buffer.BlockCopy(buffer, buffer.Length - TagSize, tag, 0, TagSize);

            using var aes = new AesGcm(_encryptionKey);
            var plaintext = new byte[cypherBytes.Length];

            aes.Decrypt(nonce, cypherBytes, tag, null, plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }
        catch (Exception ex)
        {
            throw new CryptographicException("Decryption failed", ex);
        }
    }

    /// <summary>
    /// Hash plaintext using PBKDF2 (password-based).
    /// </summary>
    public string Hash(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
            throw new ArgumentNullException(nameof(plaintext));

        try
        {
            const int iterations = 10000;
            const int saltSize = 16;

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[saltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                plaintext,
                salt,
                iterations,
                HashAlgorithmName.SHA256);

            var hash = pbkdf2.GetBytes(32);

            // Store salt + hash for verification
            var result = new byte[saltSize + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, saltSize);
            Buffer.BlockCopy(hash, 0, result, saltSize, hash.Length);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            throw new CryptographicException("Hashing failed", ex);
        }
    }

    /// <summary>
    /// Verify plaintext against PBKDF2 hash.
    /// </summary>
    public bool VerifyHash(string plaintext, string hash)
    {
        if (string.IsNullOrEmpty(plaintext) || string.IsNullOrEmpty(hash))
            return false;

        try
        {
            var buffer = Convert.FromBase64String(hash);
            const int saltSize = 16;

            if (buffer.Length < saltSize)
                return false;

            var salt = new byte[saltSize];
            var storedHash = new byte[buffer.Length - saltSize];

            Buffer.BlockCopy(buffer, 0, salt, 0, saltSize);
            Buffer.BlockCopy(buffer, saltSize, storedHash, 0, buffer.Length - saltSize);

            const int iterations = 10000;
            using var pbkdf2 = new Rfc2898DeriveBytes(
                plaintext,
                salt,
                iterations,
                HashAlgorithmName.SHA256);

            var computedHash = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
        catch
        {
            return false;
        }
    }
}
