namespace BuildingBlocks.Persistence.Abstractions;

/// <summary>
/// Interface for entities that support soft deletion.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets a value indicating whether the entity has been soft deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Gets the UTC timestamp when the entity was deleted, if applicable.
    /// </summary>
    DateTimeOffset? DeletedAt { get; }
}
