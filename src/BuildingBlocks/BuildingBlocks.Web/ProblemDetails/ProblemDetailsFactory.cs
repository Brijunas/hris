using System.Diagnostics;
using BuildingBlocks.Contracts.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.ProblemDetails;

/// <summary>
/// Factory for creating consistent RFC 7807 ProblemDetails responses.
/// </summary>
public static class ProblemDetailsFactory
{
    /// <summary>
    /// Creates a ProblemDetails from an Error object.
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateFromError(Error error, HttpContext? httpContext = null)
    {
        var (statusCode, title, type) = MapErrorTypeToStatusCode(error.Type);

        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = error.Message
        };

        problemDetails.Extensions["code"] = error.Code;

        if (httpContext != null)
        {
            problemDetails.Instance = httpContext.Request.Path;
            problemDetails.Extensions["correlationId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
        }

        if (error.Details != null)
        {
            foreach (var detail in error.Details)
            {
                problemDetails.Extensions[detail.Key] = detail.Value;
            }
        }

        return problemDetails;
    }

    /// <summary>
    /// Creates a ProblemDetails for validation errors with multiple error details.
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateValidationProblemDetails(
        IEnumerable<Error> errors,
        HttpContext? httpContext = null)
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Detail = "One or more validation errors occurred."
        };

        if (httpContext != null)
        {
            problemDetails.Instance = httpContext.Request.Path;
            problemDetails.Extensions["correlationId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
        }

        var errorList = errors.Select(e => new
        {
            code = e.Code,
            message = e.Message,
            details = e.Details
        }).ToList();

        problemDetails.Extensions["errors"] = errorList;

        return problemDetails;
    }

    /// <summary>
    /// Creates a ProblemDetails for a failed Result.
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateFromResult(Result result, HttpContext? httpContext = null)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create ProblemDetails from a successful result.");
        }

        return CreateFromError(result.Error, httpContext);
    }

    /// <summary>
    /// Creates a ProblemDetails for a failed ValidationResult with multiple errors.
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateFromValidationResult(
        ValidationResult result,
        HttpContext? httpContext = null)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create ProblemDetails from a successful result.");
        }

        return CreateValidationProblemDetails(result.Errors, httpContext);
    }

    /// <summary>
    /// Creates a generic internal server error ProblemDetails.
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateInternalServerError(
        HttpContext? httpContext = null,
        string? detail = null)
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = detail ?? "An unexpected error occurred. Please try again later."
        };

        if (httpContext != null)
        {
            problemDetails.Instance = httpContext.Request.Path;
            problemDetails.Extensions["correlationId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
        }

        return problemDetails;
    }

    /// <summary>
    /// Creates a not found ProblemDetails.
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateNotFound(
        string resource,
        string? identifier = null,
        HttpContext? httpContext = null)
    {
        var detail = identifier != null
            ? $"{resource} with identifier '{identifier}' was not found."
            : $"{resource} was not found.";

        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Detail = detail
        };

        if (httpContext != null)
        {
            problemDetails.Instance = httpContext.Request.Path;
            problemDetails.Extensions["correlationId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
        }

        return problemDetails;
    }

    /// <summary>
    /// Creates an unauthorized ProblemDetails.
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateUnauthorized(
        HttpContext? httpContext = null,
        string? detail = null)
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Detail = detail ?? "Authentication is required to access this resource."
        };

        if (httpContext != null)
        {
            problemDetails.Instance = httpContext.Request.Path;
            problemDetails.Extensions["correlationId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
        }

        return problemDetails;
    }

    /// <summary>
    /// Creates a forbidden ProblemDetails.
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateForbidden(
        HttpContext? httpContext = null,
        string? detail = null)
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Detail = detail ?? "You do not have permission to access this resource."
        };

        if (httpContext != null)
        {
            problemDetails.Instance = httpContext.Request.Path;
            problemDetails.Extensions["correlationId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
        }

        return problemDetails;
    }

    /// <summary>
    /// Creates a step-up required ProblemDetails.
    /// </summary>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateStepUpRequired(HttpContext? httpContext = null)
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Step-Up Authentication Required",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Detail = "This operation requires step-up authentication. Please re-authenticate with a second factor."
        };

        problemDetails.Extensions["requiresStepUp"] = true;

        if (httpContext != null)
        {
            problemDetails.Instance = httpContext.Request.Path;
            problemDetails.Extensions["correlationId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
        }

        return problemDetails;
    }

    private static (int StatusCode, string Title, string Type) MapErrorTypeToStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "https://tools.ietf.org/html/rfc7231#section-6.5.1"),

            ErrorType.NotFound => (
                StatusCodes.Status404NotFound,
                "Not Found",
                "https://tools.ietf.org/html/rfc7231#section-6.5.4"),

            ErrorType.Conflict => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "https://tools.ietf.org/html/rfc7231#section-6.5.8"),

            ErrorType.Unauthorized => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "https://tools.ietf.org/html/rfc7235#section-3.1"),

            ErrorType.Forbidden => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "https://tools.ietf.org/html/rfc7231#section-6.5.3"),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "https://tools.ietf.org/html/rfc7231#section-6.6.1")
        };
    }
}
