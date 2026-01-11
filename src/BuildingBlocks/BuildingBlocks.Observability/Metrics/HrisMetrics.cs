using System.Diagnostics.Metrics;

namespace BuildingBlocks.Observability.Metrics;

/// <summary>
/// Provides centralized metrics instrumentation for the HRIS application.
/// </summary>
public sealed class HrisMetrics : IDisposable
{
    /// <summary>
    /// The name of the meter.
    /// </summary>
    public const string MeterName = "Hris.Application";

    private readonly Meter _meter;

    private readonly Counter<long> _requestCounter;
    private readonly Counter<long> _operationCounter;
    private readonly Counter<long> _errorCounter;
    private readonly Histogram<double> _operationDuration;
    private readonly Counter<long> _commandCounter;
    private readonly Counter<long> _queryCounter;
    private readonly Counter<long> _eventCounter;
    private readonly UpDownCounter<long> _activeOperations;

    /// <summary>
    /// Initializes a new instance of the <see cref="HrisMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The meter factory to create meters.</param>
    public HrisMetrics(IMeterFactory meterFactory)
    {
        ArgumentNullException.ThrowIfNull(meterFactory);

        _meter = meterFactory.Create(MeterName, "1.0.0");

        _requestCounter = _meter.CreateCounter<long>(
            "hris.requests.total",
            unit: "{requests}",
            description: "Total number of HTTP requests received");

        _operationCounter = _meter.CreateCounter<long>(
            "hris.operations.total",
            unit: "{operations}",
            description: "Total number of operations executed");

        _errorCounter = _meter.CreateCounter<long>(
            "hris.errors.total",
            unit: "{errors}",
            description: "Total number of errors occurred");

        _operationDuration = _meter.CreateHistogram<double>(
            "hris.operation.duration",
            unit: "ms",
            description: "Duration of operations in milliseconds");

        _commandCounter = _meter.CreateCounter<long>(
            "hris.commands.total",
            unit: "{commands}",
            description: "Total number of commands executed");

        _queryCounter = _meter.CreateCounter<long>(
            "hris.queries.total",
            unit: "{queries}",
            description: "Total number of queries executed");

        _eventCounter = _meter.CreateCounter<long>(
            "hris.events.total",
            unit: "{events}",
            description: "Total number of events processed");

        _activeOperations = _meter.CreateUpDownCounter<long>(
            "hris.operations.active",
            unit: "{operations}",
            description: "Number of currently active operations");
    }

    /// <summary>
    /// Records a request.
    /// </summary>
    /// <param name="endpoint">The endpoint name.</param>
    /// <param name="method">The HTTP method.</param>
    /// <param name="statusCode">The response status code.</param>
    public void RecordRequest(string endpoint, string method, int statusCode)
    {
        _requestCounter.Add(1,
            new KeyValuePair<string, object?>("endpoint", endpoint),
            new KeyValuePair<string, object?>("method", method),
            new KeyValuePair<string, object?>("status_code", statusCode));
    }

    /// <summary>
    /// Records an operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="module">The module name.</param>
    /// <param name="success">Whether the operation was successful.</param>
    public void RecordOperation(string operationName, string module, bool success = true)
    {
        _operationCounter.Add(1,
            new KeyValuePair<string, object?>("operation", operationName),
            new KeyValuePair<string, object?>("module", module),
            new KeyValuePair<string, object?>("success", success));
    }

    /// <summary>
    /// Records an error.
    /// </summary>
    /// <param name="errorType">The type of error.</param>
    /// <param name="module">The module name.</param>
    /// <param name="operation">The operation where the error occurred.</param>
    public void RecordError(string errorType, string module, string? operation = null)
    {
        _errorCounter.Add(1,
            new KeyValuePair<string, object?>("error_type", errorType),
            new KeyValuePair<string, object?>("module", module),
            new KeyValuePair<string, object?>("operation", operation ?? "unknown"));
    }

    /// <summary>
    /// Records operation duration.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="durationMs">The duration in milliseconds.</param>
    /// <param name="module">The module name.</param>
    public void RecordOperationDuration(string operationName, double durationMs, string module)
    {
        _operationDuration.Record(durationMs,
            new KeyValuePair<string, object?>("operation", operationName),
            new KeyValuePair<string, object?>("module", module));
    }

    /// <summary>
    /// Records a command execution.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <param name="module">The module name.</param>
    /// <param name="success">Whether the command was successful.</param>
    public void RecordCommand(string commandName, string module, bool success = true)
    {
        _commandCounter.Add(1,
            new KeyValuePair<string, object?>("command", commandName),
            new KeyValuePair<string, object?>("module", module),
            new KeyValuePair<string, object?>("success", success));
    }

    /// <summary>
    /// Records a query execution.
    /// </summary>
    /// <param name="queryName">The name of the query.</param>
    /// <param name="module">The module name.</param>
    /// <param name="success">Whether the query was successful.</param>
    public void RecordQuery(string queryName, string module, bool success = true)
    {
        _queryCounter.Add(1,
            new KeyValuePair<string, object?>("query", queryName),
            new KeyValuePair<string, object?>("module", module),
            new KeyValuePair<string, object?>("success", success));
    }

    /// <summary>
    /// Records an event.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="module">The module name.</param>
    /// <param name="success">Whether the event was processed successfully.</param>
    public void RecordEvent(string eventName, string module, bool success = true)
    {
        _eventCounter.Add(1,
            new KeyValuePair<string, object?>("event", eventName),
            new KeyValuePair<string, object?>("module", module),
            new KeyValuePair<string, object?>("success", success));
    }

    /// <summary>
    /// Increments the active operations counter.
    /// </summary>
    /// <param name="operationType">The type of operation.</param>
    public void IncrementActiveOperations(string operationType)
    {
        _activeOperations.Add(1,
            new KeyValuePair<string, object?>("operation_type", operationType));
    }

    /// <summary>
    /// Decrements the active operations counter.
    /// </summary>
    /// <param name="operationType">The type of operation.</param>
    public void DecrementActiveOperations(string operationType)
    {
        _activeOperations.Add(-1,
            new KeyValuePair<string, object?>("operation_type", operationType));
    }

    /// <summary>
    /// Creates a scope that tracks the duration of an operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="module">The module name.</param>
    /// <returns>A disposable scope that records duration on disposal.</returns>
    public OperationScope BeginOperation(string operationName, string module)
    {
        return new OperationScope(this, operationName, module);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _meter.Dispose();
    }

    /// <summary>
    /// Represents a scope for tracking operation duration.
    /// </summary>
    public readonly struct OperationScope : IDisposable
    {
        private readonly HrisMetrics _metrics;
        private readonly string _operationName;
        private readonly string _module;
        private readonly long _startTimestamp;

        internal OperationScope(HrisMetrics metrics, string operationName, string module)
        {
            _metrics = metrics;
            _operationName = operationName;
            _module = module;
            _startTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
            _metrics.IncrementActiveOperations(operationName);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var elapsed = System.Diagnostics.Stopwatch.GetElapsedTime(_startTimestamp);
            _metrics.RecordOperationDuration(_operationName, elapsed.TotalMilliseconds, _module);
            _metrics.DecrementActiveOperations(_operationName);
        }
    }
}
