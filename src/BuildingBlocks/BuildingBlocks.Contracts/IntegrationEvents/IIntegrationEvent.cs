namespace BuildingBlocks.Contracts.IntegrationEvents;

/// <summary>
/// Marker interface for integration events used for cross-module communication
/// in the modular monolith architecture.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Unique identifier for this event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// UTC timestamp when the event occurred.
    /// </summary>
    DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Message headers containing correlation and actor context.
    /// </summary>
    MessageHeaders Headers { get; }
}
