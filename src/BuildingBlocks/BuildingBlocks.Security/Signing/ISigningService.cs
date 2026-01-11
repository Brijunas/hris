namespace BuildingBlocks.Security.Signing;

/// <summary>
/// Service for cryptographic signing and verification using Ed25519.
/// Used for audit checkpoint signing to ensure non-repudiation.
/// </summary>
public interface ISigningService
{
    /// <summary>
    /// Signs the provided data using Ed25519.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="privateKey">The 32-byte Ed25519 private key seed.</param>
    /// <returns>A 64-byte Ed25519 signature.</returns>
    byte[] Sign(byte[] data, byte[] privateKey);

    /// <summary>
    /// Verifies an Ed25519 signature against the provided data and public key.
    /// </summary>
    /// <param name="data">The original data that was signed.</param>
    /// <param name="signature">The 64-byte signature to verify.</param>
    /// <param name="publicKey">The 32-byte Ed25519 public key.</param>
    /// <returns>True if the signature is valid; false otherwise.</returns>
    bool Verify(byte[] data, byte[] signature, byte[] publicKey);

    /// <summary>
    /// Generates a new Ed25519 key pair for signing operations.
    /// </summary>
    /// <returns>
    /// A tuple containing the 32-byte private key seed and 32-byte public key.
    /// The private key should be securely stored and never exposed.
    /// </returns>
    (byte[] privateKey, byte[] publicKey) GenerateKeyPair();
}
