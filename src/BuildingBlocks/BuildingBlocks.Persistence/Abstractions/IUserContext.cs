namespace BuildingBlocks.Persistence.Abstractions;

/// <summary>
/// Provides the current user context for RLS and multi-tenancy.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets the current user's unique identifier.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Gets the current user's organizational unit identifier.
    /// </summary>
    Guid OrgUnitId { get; }

    /// <summary>
    /// Gets a value indicating whether step-up authentication is active.
    /// </summary>
    bool IsStepUpActive { get; }
}
