namespace EHRPlatform.Common.Security;

/// <summary>
/// Service for encrypting and decrypting sensitive data.
/// Used for HIPAA-compliant protection of PII at rest.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypt plaintext string using configured key.
    /// </summary>
    /// <param name="plaintext">Data to encrypt</param>
    /// <returns>Encrypted string (Base64 encoded)</returns>
    string Encrypt(string plaintext);

    /// <summary>
    /// Decrypt encrypted string using configured key.
    /// </summary>
    /// <param name="ciphertext">Encrypted data (Base64 encoded)</param>
    /// <returns>Decrypted plaintext</returns>
    string Decrypt(string ciphertext);

    /// <summary>
    /// Hash a plaintext string (one-way).
    /// Used for passwords, sensitive fields that don't need decryption.
    /// </summary>
    /// <param name="plaintext">Data to hash</param>
    /// <returns>Hashed value (Base64 encoded)</returns>
    string Hash(string plaintext);

    /// <summary>
    /// Verify plaintext against hash.
    /// </summary>
    /// <param name="plaintext">Data to verify</param>
    /// <param name="hash">Hash to compare against</param>
    /// <returns>True if plaintext matches hash</returns>
    bool VerifyHash(string plaintext, string hash);
}
