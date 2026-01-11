namespace BuildingBlocks.Security.Password;

/// <summary>
/// Service for secure password hashing using Argon2id.
/// Provides memory-hard hashing resistant to GPU and ASIC attacks.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password using Argon2id with secure default parameters.
    /// </summary>
    /// <param name="password">The plaintext password to hash.</param>
    /// <returns>
    /// A PHC (Password Hashing Competition) format string containing
    /// the algorithm, parameters, salt, and hash.
    /// Example: $argon2id$v=19$m=65536,t=3,p=1$salt$hash
    /// </returns>
    string Hash(string password);

    /// <summary>
    /// Verifies a password against a previously computed hash.
    /// </summary>
    /// <param name="password">The plaintext password to verify.</param>
    /// <param name="hash">The PHC format hash string to verify against.</param>
    /// <returns>True if the password matches the hash; false otherwise.</returns>
    bool Verify(string password, string hash);
}
