namespace BuildingBlocks.Contracts.Results;

/// <summary>
/// Extension methods for Result types enabling functional composition.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps a successful result to a new result using the specified function.
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result<TOut>.Success(mapper(result.Value))
            : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    /// Maps a successful result to a new result using an async function.
    /// </summary>
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<TOut>> mapper)
    {
        return result.IsSuccess
            ? Result<TOut>.Success(await mapper(result.Value))
            : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    /// Chains result-returning operations (flatMap/bind).
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> binder)
    {
        return result.IsSuccess
            ? binder(result.Value)
            : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    /// Chains async result-returning operations.
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> binder)
    {
        return result.IsSuccess
            ? await binder(result.Value)
            : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    /// Chains async result-returning operations on a Task result.
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> binder)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? await binder(result.Value)
            : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    /// Executes an action on success without changing the result.
    /// </summary>
    public static Result<TValue> Tap<TValue>(
        this Result<TValue> result,
        Action<TValue> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Executes an async action on success without changing the result.
    /// </summary>
    public static async Task<Result<TValue>> TapAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, Task> action)
    {
        if (result.IsSuccess)
        {
            await action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Returns the value on success or the specified default on failure.
    /// </summary>
    public static TValue GetValueOrDefault<TValue>(
        this Result<TValue> result,
        TValue defaultValue)
    {
        return result.IsSuccess ? result.Value : defaultValue;
    }

    /// <summary>
    /// Returns the value on success or invokes the factory on failure.
    /// </summary>
    public static TValue GetValueOrDefault<TValue>(
        this Result<TValue> result,
        Func<Error, TValue> defaultFactory)
    {
        return result.IsSuccess ? result.Value : defaultFactory(result.Error);
    }

    /// <summary>
    /// Matches the result to one of two functions based on success/failure.
    /// </summary>
    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error);
    }

    /// <summary>
    /// Matches the result to one of two async functions.
    /// </summary>
    public static async Task<TOut> MatchAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<TOut>> onSuccess,
        Func<Error, Task<TOut>> onFailure)
    {
        return result.IsSuccess
            ? await onSuccess(result.Value)
            : await onFailure(result.Error);
    }

    /// <summary>
    /// Ensures a condition is met, returning a failure if not.
    /// </summary>
    public static Result<TValue> Ensure<TValue>(
        this Result<TValue> result,
        Func<TValue, bool> predicate,
        Error error)
    {
        if (result.IsFailure)
        {
            return result;
        }

        return predicate(result.Value)
            ? result
            : Result<TValue>.Failure(error);
    }

    /// <summary>
    /// Combines multiple results, returning the first failure or all values on success.
    /// </summary>
    public static Result<IReadOnlyList<TValue>> Combine<TValue>(
        params Result<TValue>[] results)
    {
        var failures = results.Where(r => r.IsFailure).ToList();

        if (failures.Count > 0)
        {
            return Result<IReadOnlyList<TValue>>.Failure(failures[0].Error);
        }

        return Result<IReadOnlyList<TValue>>.Success(
            results.Select(r => r.Value).ToList());
    }

    /// <summary>
    /// Converts a nullable value to a Result, using NotFound error if null.
    /// </summary>
    public static Result<TValue> ToResult<TValue>(
        this TValue? value,
        Error errorIfNull) where TValue : class
    {
        return value is null
            ? Result<TValue>.Failure(errorIfNull)
            : Result<TValue>.Success(value);
    }

    /// <summary>
    /// Converts a nullable value type to a Result.
    /// </summary>
    public static Result<TValue> ToResult<TValue>(
        this TValue? value,
        Error errorIfNull) where TValue : struct
    {
        return value.HasValue
            ? Result<TValue>.Success(value.Value)
            : Result<TValue>.Failure(errorIfNull);
    }
}
