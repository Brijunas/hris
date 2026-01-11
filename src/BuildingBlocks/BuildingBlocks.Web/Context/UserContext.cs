using BuildingBlocks.Persistence.Abstractions;

namespace BuildingBlocks.Web.Context;

/// <summary>
/// Implementation of IUserContext that provides the current user context
/// extracted from HTTP requests (JWT claims or headers).
/// Scoped lifetime - created per HTTP request.
/// </summary>
public sealed class UserContext : IUserContext
{
    /// <summary>
    /// Gets or sets the current user's unique identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the current user's organizational unit identifier.
    /// </summary>
    public Guid OrgUnitId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether step-up authentication is active.
    /// </summary>
    public bool IsStepUpActive { get; set; }
}
