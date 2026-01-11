using BuildingBlocks.Contracts.Results;
using BuildingBlocks.Web.ProblemDetails;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Extensions;

/// <summary>
/// Extension methods for converting Result objects to HTTP responses.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result to an appropriate IResult for minimal APIs.
    /// </summary>
    public static IResult ToHttpResult(this Result result, HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        var problemDetails = ProblemDetailsFactory.CreateFromError(result.Error, httpContext);
        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Converts a Result<T> to an appropriate IResult for minimal APIs.
    /// </summary>
    public static IResult ToHttpResult<T>(this Result<T> result, HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        var problemDetails = ProblemDetailsFactory.CreateFromError(result.Error, httpContext);
        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Converts a Result<T> to a Created response on success.
    /// </summary>
    public static IResult ToCreatedResult<T>(
        this Result<T> result,
        HttpContext httpContext,
        string? routeName = null,
        object? routeValues = null)
    {
        if (result.IsSuccess)
        {
            if (routeName != null)
            {
                return Results.CreatedAtRoute(routeName, routeValues, result.Value);
            }
            return Results.Created((string?)null, result.Value);
        }

        var problemDetails = ProblemDetailsFactory.CreateFromError(result.Error, httpContext);
        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Converts a Result to a Created response on success (for operations returning identifier).
    /// </summary>
    public static IResult ToCreatedResult(
        this Result result,
        HttpContext httpContext,
        string location)
    {
        if (result.IsSuccess)
        {
            return Results.Created(location, null);
        }

        var problemDetails = ProblemDetailsFactory.CreateFromError(result.Error, httpContext);
        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Converts a ValidationResult to an appropriate IResult for minimal APIs.
    /// </summary>
    public static IResult ToHttpResult(this ValidationResult result, HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        var problemDetails = ProblemDetailsFactory.CreateValidationProblemDetails(result.Errors, httpContext);
        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Converts a ValidationResult<T> to an appropriate IResult for minimal APIs.
    /// </summary>
    public static IResult ToHttpResult<T>(this ValidationResult<T> result, HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        var problemDetails = ProblemDetailsFactory.CreateValidationProblemDetails(result.Errors, httpContext);
        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Matches on Result and executes the appropriate function.
    /// </summary>
    public static IResult Match(
        this Result result,
        HttpContext httpContext,
        Func<IResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess();
        }

        var problemDetails = ProblemDetailsFactory.CreateFromError(result.Error, httpContext);
        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Matches on Result<T> and executes the appropriate function.
    /// </summary>
    public static IResult Match<T>(
        this Result<T> result,
        HttpContext httpContext,
        Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value);
        }

        var problemDetails = ProblemDetailsFactory.CreateFromError(result.Error, httpContext);
        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Matches on Result<T> and executes the appropriate function with custom error handling.
    /// </summary>
    public static IResult Match<T>(
        this Result<T> result,
        Func<T, IResult> onSuccess,
        Func<Error, IResult> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error);
    }
}
