namespace BuildingBlocks.Contracts.Results;

/// <summary>
/// Represents a validation result that can contain multiple errors.
/// Used for operations where multiple validation failures should be reported.
/// </summary>
public sealed class ValidationResult : Result
{
    private ValidationResult(Error[] errors)
        : base(false, errors[0])
    {
        Errors = errors;
    }

    private ValidationResult()
        : base(true, Error.None)
    {
        Errors = [];
    }

    /// <summary>
    /// All validation errors that occurred.
    /// </summary>
    public IReadOnlyList<Error> Errors { get; }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public new static ValidationResult Success() => new();

    /// <summary>
    /// Creates a failed validation result with the specified errors.
    /// </summary>
    public static ValidationResult WithErrors(params Error[] errors)
    {
        if (errors.Length == 0)
        {
            throw new ArgumentException("At least one error is required.", nameof(errors));
        }

        return new ValidationResult(errors);
    }

    /// <summary>
    /// Creates a failed validation result from a collection of errors.
    /// </summary>
    public static ValidationResult WithErrors(IEnumerable<Error> errors)
    {
        var errorArray = errors.ToArray();
        if (errorArray.Length == 0)
        {
            throw new ArgumentException("At least one error is required.", nameof(errors));
        }

        return new ValidationResult(errorArray);
    }
}

/// <summary>
/// Represents a validation result with a value that can contain multiple errors.
/// </summary>
/// <typeparam name="TValue">The type of the value on success.</typeparam>
public sealed class ValidationResult<TValue> : Result<TValue>
{
    private ValidationResult(TValue value)
        : base(value, true, Error.None)
    {
        Errors = [];
    }

    private ValidationResult(Error[] errors)
        : base(default, false, errors[0])
    {
        Errors = errors;
    }

    /// <summary>
    /// All validation errors that occurred.
    /// </summary>
    public IReadOnlyList<Error> Errors { get; }

    /// <summary>
    /// Creates a successful validation result with the specified value.
    /// </summary>
    public new static ValidationResult<TValue> Success(TValue value) => new(value);

    /// <summary>
    /// Creates a failed validation result with the specified errors.
    /// </summary>
    public static ValidationResult<TValue> WithErrors(params Error[] errors)
    {
        if (errors.Length == 0)
        {
            throw new ArgumentException("At least one error is required.", nameof(errors));
        }

        return new ValidationResult<TValue>(errors);
    }

    /// <summary>
    /// Creates a failed validation result from a collection of errors.
    /// </summary>
    public static ValidationResult<TValue> WithErrors(IEnumerable<Error> errors)
    {
        var errorArray = errors.ToArray();
        if (errorArray.Length == 0)
        {
            throw new ArgumentException("At least one error is required.", nameof(errors));
        }

        return new ValidationResult<TValue>(errorArray);
    }
}
