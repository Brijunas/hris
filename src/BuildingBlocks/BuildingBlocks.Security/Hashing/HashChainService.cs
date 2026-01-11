using System.Security.Cryptography;
using System.Text;

namespace BuildingBlocks.Security.Hashing;

/// <summary>
/// Implementation of <see cref="IHashChainService"/> using SHA-256.
/// Creates tamper-evident audit chains by cryptographically linking entries.
/// </summary>
public sealed class HashChainService : IHashChainService
{
    /// <inheritdoc />
    public byte[] ComputeHash(byte[]? previousHash, byte[] payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        // Combine previous hash (or empty) with payload
        // Format: [previousHash (32 bytes or 0 bytes)][payload]
        var previousHashLength = previousHash?.Length ?? 0;
        var combinedLength = previousHashLength + payload.Length;
        var combined = new byte[combinedLength];

        if (previousHash is not null)
        {
            Buffer.BlockCopy(previousHash, 0, combined, 0, previousHash.Length);
        }

        Buffer.BlockCopy(payload, 0, combined, previousHashLength, payload.Length);

        return SHA256.HashData(combined);
    }

    /// <inheritdoc />
    public byte[] ComputeHash(byte[]? previousHash, string canonicalPayload)
    {
        ArgumentNullException.ThrowIfNull(canonicalPayload);

        var payloadBytes = Encoding.UTF8.GetBytes(canonicalPayload);
        return ComputeHash(previousHash, payloadBytes);
    }

    /// <inheritdoc />
    public bool VerifyChain(IEnumerable<(byte[] payload, byte[] expectedHash)> chain)
    {
        ArgumentNullException.ThrowIfNull(chain);

        byte[]? previousHash = null;

        foreach (var (payload, expectedHash) in chain)
        {
            if (payload is null || expectedHash is null)
            {
                return false;
            }

            var computedHash = ComputeHash(previousHash, payload);

            if (!CryptographicOperations.FixedTimeEquals(computedHash, expectedHash))
            {
                return false;
            }

            previousHash = expectedHash;
        }

        return true;
    }
}
