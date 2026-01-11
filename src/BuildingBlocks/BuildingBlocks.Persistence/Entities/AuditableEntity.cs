using BuildingBlocks.Persistence.Abstractions;

namespace BuildingBlocks.Persistence.Entities;

/// <summary>
/// Base class for entities that track creation and modification timestamps.
/// </summary>
public abstract class AuditableEntity : Entity, IAuditableEntity
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Sets the creation timestamp. Should only be called when the entity is first created.
    /// </summary>
    public void SetCreatedAt(DateTimeOffset timestamp)
    {
        CreatedAt = timestamp;
        UpdatedAt = timestamp;
    }

    /// <summary>
    /// Updates the modification timestamp.
    /// </summary>
    public void SetUpdatedAt(DateTimeOffset timestamp)
    {
        UpdatedAt = timestamp;
    }
}
