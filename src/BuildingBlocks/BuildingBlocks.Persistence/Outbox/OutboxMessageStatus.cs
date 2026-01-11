namespace BuildingBlocks.Persistence.Outbox;

/// <summary>
/// Represents the processing status of an outbox message.
/// </summary>
public enum OutboxMessageStatus
{
    /// <summary>
    /// The message is pending and awaiting processing.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The message is currently being processed.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// The message has been successfully processed.
    /// </summary>
    Processed = 2,

    /// <summary>
    /// The message processing has failed after all retry attempts.
    /// </summary>
    Failed = 3
}
