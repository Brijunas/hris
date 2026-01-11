using BuildingBlocks.Persistence.Abstractions;

namespace BuildingBlocks.Persistence.Entities;

/// <summary>
/// Base class for entities that support soft deletion with audit tracking.
/// </summary>
public abstract class SoftDeletableEntity : AuditableEntity, ISoftDeletable
{
    /// <inheritdoc />
    public bool IsDeleted { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <summary>
    /// Marks the entity as deleted.
    /// </summary>
    public void MarkAsDeleted(DateTimeOffset timestamp)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedAt = timestamp;
        SetUpdatedAt(timestamp);
    }

    /// <summary>
    /// Restores a soft-deleted entity.
    /// </summary>
    public void Restore(DateTimeOffset timestamp)
    {
        if (!IsDeleted)
        {
            return;
        }

        IsDeleted = false;
        DeletedAt = null;
        SetUpdatedAt(timestamp);
    }
}
