namespace BuildingBlocks.Persistence.Outbox;

/// <summary>
/// Represents the processing status of an inbox message.
/// </summary>
public enum InboxMessageStatus
{
    /// <summary>
    /// The message has been received but not yet processed.
    /// </summary>
    Received = 0,

    /// <summary>
    /// The message has been successfully processed.
    /// </summary>
    Processed = 1,

    /// <summary>
    /// The message processing has failed.
    /// </summary>
    Failed = 2
}
