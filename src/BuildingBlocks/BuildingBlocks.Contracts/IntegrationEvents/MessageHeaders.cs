namespace BuildingBlocks.Contracts.IntegrationEvents;

/// <summary>
/// Message headers for distributed tracing and actor context propagation
/// across module boundaries in the modular monolith.
/// </summary>
public sealed record MessageHeaders
{
    /// <summary>
    /// Correlation ID for tracking a request across multiple operations.
    /// All events triggered by the same user action share this ID.
    /// </summary>
    public required Guid CorrelationId { get; init; }

    /// <summary>
    /// Causation ID linking this event to the event or command that caused it.
    /// Forms a chain of causality for debugging and auditing.
    /// </summary>
    public Guid? CausationId { get; init; }

    /// <summary>
    /// The user ID of the actor who initiated the action.
    /// Used for audit trails and authorization checks.
    /// </summary>
    public Guid? ActorUserId { get; init; }

    /// <summary>
    /// The organizational unit ID of the actor.
    /// Used for multi-tenant and hierarchical access control.
    /// </summary>
    public Guid? ActorOrgUnitId { get; init; }

    /// <summary>
    /// Creates headers with a new correlation ID for a new operation chain.
    /// </summary>
    public static MessageHeaders Create(Guid? actorUserId = null, Guid? actorOrgUnitId = null) =>
        new()
        {
            CorrelationId = Guid.NewGuid(),
            ActorUserId = actorUserId,
            ActorOrgUnitId = actorOrgUnitId
        };

    /// <summary>
    /// Creates child headers preserving correlation context with new causation.
    /// </summary>
    /// <param name="causationId">The event ID that caused this new event.</param>
    public MessageHeaders CreateChild(Guid causationId) =>
        this with { CausationId = causationId };
}
