namespace BuildingBlocks.Persistence.Outbox;

/// <summary>
/// Represents an inbox message for idempotent message handling.
/// Ensures that messages are processed exactly once per handler.
/// </summary>
public sealed class InboxMessage
{
    /// <summary>
    /// Gets the unique identifier of the original message.
    /// This serves as the primary key and deduplication identifier.
    /// </summary>
    public Guid MessageId { get; private init; }

    /// <summary>
    /// Gets the UTC timestamp when the message was received.
    /// </summary>
    public DateTimeOffset ReceivedAt { get; private init; }

    /// <summary>
    /// Gets the fully qualified name of the handler processing this message.
    /// Combined with MessageId, ensures a message is processed once per handler.
    /// </summary>
    public string Handler { get; private init; } = string.Empty;

    /// <summary>
    /// Gets the UTC timestamp when the message was successfully processed.
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; private set; }

    /// <summary>
    /// Gets the current processing status of the message.
    /// </summary>
    public InboxMessageStatus Status { get; private set; }

    /// <summary>
    /// Gets the error message if processing failed.
    /// </summary>
    public string? Error { get; private set; }

    private InboxMessage()
    {
        // Required by EF Core
    }

    /// <summary>
    /// Creates a new inbox message record.
    /// </summary>
    public static InboxMessage Create(
        Guid messageId,
        string handler,
        DateTimeOffset receivedAt)
    {
        return new InboxMessage
        {
            MessageId = messageId,
            Handler = handler,
            ReceivedAt = receivedAt,
            Status = InboxMessageStatus.Received
        };
    }

    /// <summary>
    /// Marks the message as successfully processed.
    /// </summary>
    public void MarkAsProcessed(DateTimeOffset processedAt)
    {
        Status = InboxMessageStatus.Processed;
        ProcessedAt = processedAt;
        Error = null;
    }

    /// <summary>
    /// Marks the message as failed with an error.
    /// </summary>
    public void MarkAsFailed(string error)
    {
        Status = InboxMessageStatus.Failed;
        Error = error;
    }
}
