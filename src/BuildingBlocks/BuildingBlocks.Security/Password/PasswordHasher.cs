using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace BuildingBlocks.Security.Password;

/// <summary>
/// Implementation of <see cref="IPasswordHasher"/> using Argon2id.
/// Uses OWASP-recommended parameters for secure password storage.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    // OWASP recommended parameters for Argon2id (as of 2024)
    // See: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
    private const int MemorySize = 65536; // 64 MB in KB
    private const int Iterations = 3;
    private const int DegreeOfParallelism = 1;
    private const int HashLength = 32;
    private const int SaltLength = 16;
    private const int Argon2Version = 19; // 0x13

    /// <inheritdoc />
    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        var hash = ComputeArgon2Hash(password, salt);

        // Format as PHC string: $argon2id$v=19$m=65536,t=3,p=1$<salt>$<hash>
        var saltBase64 = Convert.ToBase64String(salt)
            .TrimEnd('=')
            .Replace('+', '.')
            .Replace('/', '_');

        var hashBase64 = Convert.ToBase64String(hash)
            .TrimEnd('=')
            .Replace('+', '.')
            .Replace('/', '_');

        return $"$argon2id$v={Argon2Version}$m={MemorySize},t={Iterations},p={DegreeOfParallelism}${saltBase64}${hashBase64}";
    }

    /// <inheritdoc />
    public bool Verify(string password, string hash)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        ArgumentException.ThrowIfNullOrEmpty(hash);

        try
        {
            var parsed = ParsePhcString(hash);
            if (parsed is null)
            {
                return false;
            }

            var (memorySize, iterations, parallelism, salt, expectedHash) = parsed.Value;

            var computedHash = ComputeArgon2Hash(password, salt, memorySize, iterations, parallelism);

            return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static byte[] ComputeArgon2Hash(
        string password,
        byte[] salt,
        int memorySize = MemorySize,
        int iterations = Iterations,
        int parallelism = DegreeOfParallelism)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var argon2 = new Argon2id(passwordBytes)
        {
            Salt = salt,
            MemorySize = memorySize,
            Iterations = iterations,
            DegreeOfParallelism = parallelism
        };

        return argon2.GetBytes(HashLength);
    }

    private static (int memorySize, int iterations, int parallelism, byte[] salt, byte[] hash)? ParsePhcString(string phc)
    {
        // Expected format: $argon2id$v=19$m=65536,t=3,p=1$<salt>$<hash>
        var parts = phc.Split('$', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 5)
        {
            return null;
        }

        // Validate algorithm
        if (parts[0] != "argon2id")
        {
            return null;
        }

        // Validate version
        if (!parts[1].StartsWith("v=", StringComparison.Ordinal))
        {
            return null;
        }

        // Parse parameters
        var paramParts = parts[2].Split(',');
        if (paramParts.Length != 3)
        {
            return null;
        }

        int memorySize = 0, iterations = 0, parallelism = 0;

        foreach (var param in paramParts)
        {
            var kv = param.Split('=');
            if (kv.Length != 2)
            {
                return null;
            }

            switch (kv[0])
            {
                case "m":
                    memorySize = int.Parse(kv[1], CultureInfo.InvariantCulture);
                    break;
                case "t":
                    iterations = int.Parse(kv[1], CultureInfo.InvariantCulture);
                    break;
                case "p":
                    parallelism = int.Parse(kv[1], CultureInfo.InvariantCulture);
                    break;
                default:
                    return null;
            }
        }

        // Decode salt and hash (modified base64 with . instead of + and _ instead of /)
        var salt = DecodeBase64(parts[3]);
        var hash = DecodeBase64(parts[4]);

        return (memorySize, iterations, parallelism, salt, hash);
    }

    private static byte[] DecodeBase64(string encoded)
    {
        // Restore standard Base64 characters and padding
        var standardBase64 = encoded
            .Replace('.', '+')
            .Replace('_', '/');

        // Add padding if needed
        var padLength = (4 - (standardBase64.Length % 4)) % 4;
        standardBase64 += new string('=', padLength);

        return Convert.FromBase64String(standardBase64);
    }
}
