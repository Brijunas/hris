using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BuildingBlocks.Security.Serialization;

/// <summary>
/// Implementation of <see cref="ICanonicalJsonSerializer"/> using System.Text.Json.
/// Produces deterministic JSON by sorting keys alphabetically and removing whitespace.
/// </summary>
public sealed class CanonicalJsonSerializer : ICanonicalJsonSerializer
{
    private static readonly JsonSerializerOptions SerializeOptions = new()
    {
        // Use minimal encoding - don't escape characters unnecessarily
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // Ensure consistent property naming
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // No pretty printing
        WriteIndented = false
    };

    private static readonly JsonSerializerOptions CanonicalWriteOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false
    };

    /// <inheritdoc />
    public string Serialize<T>(T obj)
    {
        if (obj is null)
        {
            return "null";
        }

        // First serialize to JSON string, then parse to JsonNode for sorting
        var json = JsonSerializer.Serialize(obj, SerializeOptions);
        var node = JsonNode.Parse(json);

        if (node is null)
        {
            return "null";
        }

        // Sort all object properties recursively
        var sorted = SortProperties(node);

        return sorted?.ToJsonString(CanonicalWriteOptions) ?? "null";
    }

    private static JsonNode? SortProperties(JsonNode? node)
    {
        return node switch
        {
            JsonObject obj => SortObject(obj),
            JsonArray arr => SortArray(arr),
            _ => node?.DeepClone()
        };
    }

    private static JsonObject SortObject(JsonObject obj)
    {
        var sorted = new JsonObject();

        // Get all properties, sort by key, then add to new object
        var sortedProperties = obj
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
            .ToList();

        foreach (var kvp in sortedProperties)
        {
            sorted[kvp.Key] = SortProperties(kvp.Value);
        }

        return sorted;
    }

    private static JsonArray SortArray(JsonArray arr)
    {
        var sorted = new JsonArray();

        foreach (var item in arr)
        {
            sorted.Add(SortProperties(item));
        }

        return sorted;
    }
}
