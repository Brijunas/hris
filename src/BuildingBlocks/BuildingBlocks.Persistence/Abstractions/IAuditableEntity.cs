namespace BuildingBlocks.Persistence.Abstractions;

/// <summary>
/// Interface for entities that track creation and modification timestamps.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets the UTC timestamp when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the UTC timestamp when the entity was last updated.
    /// </summary>
    DateTimeOffset UpdatedAt { get; }
}
