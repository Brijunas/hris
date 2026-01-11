namespace BuildingBlocks.Contracts.IntegrationEvents;

/// <summary>
/// Base record for all integration events providing common metadata.
/// Uses record types for immutability and value-based equality.
/// </summary>
public abstract record IntegrationEvent : IIntegrationEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <inheritdoc />
    public required MessageHeaders Headers { get; init; }
}
