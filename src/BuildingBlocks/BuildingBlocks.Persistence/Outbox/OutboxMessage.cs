namespace BuildingBlocks.Persistence.Outbox;

/// <summary>
/// Represents an outbox message for the transactional outbox pattern.
/// Ensures reliable message publishing with at-least-once delivery semantics.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>
    /// Gets the unique identifier for this outbox message.
    /// </summary>
    public Guid Id { get; private init; }

    /// <summary>
    /// Gets the UTC timestamp when the event occurred.
    /// </summary>
    public DateTimeOffset OccurredAt { get; private init; }

    /// <summary>
    /// Gets the fully qualified type name of the message.
    /// </summary>
    public string Type { get; private init; } = string.Empty;

    /// <summary>
    /// Gets the JSON-serialized message payload.
    /// </summary>
    public string Payload { get; private init; } = string.Empty;

    /// <summary>
    /// Gets the JSON-serialized message headers (correlation ID, causation ID, etc.).
    /// </summary>
    public string Headers { get; private init; } = string.Empty;

    /// <summary>
    /// Gets the current processing status of the message.
    /// </summary>
    public OutboxMessageStatus Status { get; private set; }

    /// <summary>
    /// Gets the number of processing attempts made.
    /// </summary>
    public int Attempts { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp for the next retry attempt, if applicable.
    /// </summary>
    public DateTimeOffset? NextAttemptAt { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the message was successfully processed.
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; private set; }

    /// <summary>
    /// Gets the error message from the last failed processing attempt.
    /// </summary>
    public string? LastError { get; private set; }

    private OutboxMessage()
    {
        // Required by EF Core
    }

    /// <summary>
    /// Creates a new outbox message.
    /// </summary>
    public static OutboxMessage Create(
        Guid id,
        string type,
        string payload,
        string headers,
        DateTimeOffset occurredAt)
    {
        return new OutboxMessage
        {
            Id = id,
            Type = type,
            Payload = payload,
            Headers = headers,
            OccurredAt = occurredAt,
            Status = OutboxMessageStatus.Pending,
            Attempts = 0,
            NextAttemptAt = occurredAt
        };
    }

    /// <summary>
    /// Marks the message as being processed.
    /// </summary>
    public void MarkAsProcessing()
    {
        Status = OutboxMessageStatus.Processing;
        Attempts++;
    }

    /// <summary>
    /// Marks the message as successfully processed.
    /// </summary>
    public void MarkAsProcessed(DateTimeOffset processedAt)
    {
        Status = OutboxMessageStatus.Processed;
        ProcessedAt = processedAt;
        NextAttemptAt = null;
        LastError = null;
    }

    /// <summary>
    /// Marks the message as failed with a retry scheduled.
    /// </summary>
    public void MarkAsFailed(string error, DateTimeOffset? nextAttemptAt)
    {
        LastError = error;
        NextAttemptAt = nextAttemptAt;
        Status = nextAttemptAt.HasValue
            ? OutboxMessageStatus.Pending
            : OutboxMessageStatus.Failed;
    }
}
