namespace BuildingBlocks.Security.Hashing;

/// <summary>
/// Service for creating and verifying tamper-evident hash chains for audit trails.
/// Uses SHA-256 to create cryptographic links between sequential audit entries.
/// </summary>
public interface IHashChainService
{
    /// <summary>
    /// Computes a SHA-256 hash combining the previous hash with the current payload.
    /// This creates a cryptographic chain where each entry depends on all previous entries.
    /// </summary>
    /// <param name="previousHash">The hash of the previous entry in the chain, or null for the genesis entry.</param>
    /// <param name="payload">The raw payload bytes to include in the hash.</param>
    /// <returns>A 32-byte SHA-256 hash.</returns>
    byte[] ComputeHash(byte[]? previousHash, byte[] payload);

    /// <summary>
    /// Computes a SHA-256 hash combining the previous hash with a canonical JSON payload.
    /// The payload should be in canonical form (sorted keys, no whitespace) for consistency.
    /// </summary>
    /// <param name="previousHash">The hash of the previous entry in the chain, or null for the genesis entry.</param>
    /// <param name="canonicalPayload">The canonical JSON string representation of the payload.</param>
    /// <returns>A 32-byte SHA-256 hash.</returns>
    byte[] ComputeHash(byte[]? previousHash, string canonicalPayload);

    /// <summary>
    /// Verifies the integrity of an entire hash chain by recomputing each hash
    /// and comparing it to the expected value.
    /// </summary>
    /// <param name="chain">
    /// An ordered sequence of (payload, expectedHash) tuples representing the chain.
    /// The first entry's hash is computed with a null previous hash.
    /// </param>
    /// <returns>True if all hashes in the chain are valid; false if any tampering is detected.</returns>
    bool VerifyChain(IEnumerable<(byte[] payload, byte[] expectedHash)> chain);
}
