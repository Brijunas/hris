namespace BuildingBlocks.Observability.Correlation;

/// <summary>
/// Provides access to the correlation ID for the current request context.
/// </summary>
public interface ICorrelationIdAccessor
{
    /// <summary>
    /// Gets the correlation ID for the current context.
    /// </summary>
    /// <returns>The correlation ID, or null if not set.</returns>
    string? GetCorrelationId();

    /// <summary>
    /// Sets the correlation ID for the current context.
    /// </summary>
    /// <param name="correlationId">The correlation ID to set.</param>
    void SetCorrelationId(string correlationId);
}
