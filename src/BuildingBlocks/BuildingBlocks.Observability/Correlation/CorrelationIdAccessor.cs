namespace BuildingBlocks.Observability.Correlation;

/// <summary>
/// Thread-safe implementation of <see cref="ICorrelationIdAccessor"/> using AsyncLocal storage.
/// </summary>
public sealed class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private static readonly AsyncLocal<string?> CorrelationIdHolder = new();

    /// <inheritdoc />
    public string? GetCorrelationId() => CorrelationIdHolder.Value;

    /// <inheritdoc />
    public void SetCorrelationId(string correlationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        CorrelationIdHolder.Value = correlationId;
    }
}
