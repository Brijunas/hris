namespace BuildingBlocks.Security.Serialization;

/// <summary>
/// Service for serializing objects to canonical JSON format.
/// Produces deterministic output suitable for hashing and signatures.
/// </summary>
public interface ICanonicalJsonSerializer
{
    /// <summary>
    /// Serializes an object to canonical JSON format.
    /// The output has sorted keys and no whitespace, ensuring identical
    /// objects always produce identical JSON strings.
    /// </summary>
    /// <typeparam name="T">The type of object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>
    /// A canonical JSON string with:
    /// - Properties sorted alphabetically at all levels
    /// - No whitespace between elements
    /// - Consistent encoding for special characters
    /// </returns>
    string Serialize<T>(T obj);
}
