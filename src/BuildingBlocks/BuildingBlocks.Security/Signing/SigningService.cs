using NSec.Cryptography;

namespace BuildingBlocks.Security.Signing;

/// <summary>
/// Implementation of <see cref="ISigningService"/> using Ed25519 via NSec.
/// Provides high-performance signing for audit checkpoint verification.
/// </summary>
public sealed class SigningService : ISigningService
{
    private static readonly Ed25519 Algorithm = SignatureAlgorithm.Ed25519;

    /// <inheritdoc />
    public byte[] Sign(byte[] data, byte[] privateKey)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(privateKey);

        if (privateKey.Length != Algorithm.PrivateKeySize)
        {
            throw new ArgumentException(
                $"Private key must be {Algorithm.PrivateKeySize} bytes, but was {privateKey.Length} bytes.",
                nameof(privateKey));
        }

        using var key = Key.Import(Algorithm, privateKey, KeyBlobFormat.RawPrivateKey);
        return Algorithm.Sign(key, data);
    }

    /// <inheritdoc />
    public bool Verify(byte[] data, byte[] signature, byte[] publicKey)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(signature);
        ArgumentNullException.ThrowIfNull(publicKey);

        if (signature.Length != Algorithm.SignatureSize)
        {
            return false;
        }

        if (publicKey.Length != Algorithm.PublicKeySize)
        {
            return false;
        }

        try
        {
            var pubKey = PublicKey.Import(Algorithm, publicKey, KeyBlobFormat.RawPublicKey);
            return Algorithm.Verify(pubKey, data, signature);
        }
        catch (FormatException)
        {
            // Invalid key format
            return false;
        }
    }

    /// <inheritdoc />
    public (byte[] privateKey, byte[] publicKey) GenerateKeyPair()
    {
        using var key = Key.Create(Algorithm, new KeyCreationParameters
        {
            ExportPolicy = KeyExportPolicies.AllowPlaintextExport
        });

        var privateKeyBytes = key.Export(KeyBlobFormat.RawPrivateKey);
        var publicKeyBytes = key.PublicKey.Export(KeyBlobFormat.RawPublicKey);

        return (privateKeyBytes, publicKeyBytes);
    }
}
