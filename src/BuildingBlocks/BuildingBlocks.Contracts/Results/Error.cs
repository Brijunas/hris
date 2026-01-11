namespace BuildingBlocks.Contracts.Results;

/// <summary>
/// Represents an error with a code and message for clean error handling.
/// Immutable record type for value-based equality.
/// </summary>
public sealed record Error
{
    /// <summary>
    /// A machine-readable error code (e.g., "Personnel.NotFound").
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// A human-readable error message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// The type/category of error.
    /// </summary>
    public ErrorType Type { get; init; } = ErrorType.Failure;

    /// <summary>
    /// Additional details about the error.
    /// </summary>
    public IReadOnlyDictionary<string, object>? Details { get; init; }

    /// <summary>
    /// Creates a generic failure error.
    /// </summary>
    public static Error Failure(string code, string message) =>
        new() { Code = code, Message = message, Type = ErrorType.Failure };

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    public static Error Validation(string code, string message) =>
        new() { Code = code, Message = message, Type = ErrorType.Validation };

    /// <summary>
    /// Creates a not found error.
    /// </summary>
    public static Error NotFound(string code, string message) =>
        new() { Code = code, Message = message, Type = ErrorType.NotFound };

    /// <summary>
    /// Creates a conflict error (e.g., duplicate, concurrency).
    /// </summary>
    public static Error Conflict(string code, string message) =>
        new() { Code = code, Message = message, Type = ErrorType.Conflict };

    /// <summary>
    /// Creates an unauthorized error.
    /// </summary>
    public static Error Unauthorized(string code, string message) =>
        new() { Code = code, Message = message, Type = ErrorType.Unauthorized };

    /// <summary>
    /// Creates a forbidden error.
    /// </summary>
    public static Error Forbidden(string code, string message) =>
        new() { Code = code, Message = message, Type = ErrorType.Forbidden };

    /// <summary>
    /// Represents no error (for successful operations).
    /// </summary>
    public static readonly Error None = new() { Code = string.Empty, Message = string.Empty };

    /// <summary>
    /// Represents a null value error.
    /// </summary>
    public static readonly Error NullValue = Validation("Error.NullValue", "A null value was provided.");
}

/// <summary>
/// Classification of error types for appropriate handling.
/// </summary>
public enum ErrorType
{
    /// <summary>Generic failure.</summary>
    Failure,

    /// <summary>Input validation failure.</summary>
    Validation,

    /// <summary>Requested resource not found.</summary>
    NotFound,

    /// <summary>Conflict with current state (duplicate, concurrency).</summary>
    Conflict,

    /// <summary>Authentication required.</summary>
    Unauthorized,

    /// <summary>Insufficient permissions.</summary>
    Forbidden
}
