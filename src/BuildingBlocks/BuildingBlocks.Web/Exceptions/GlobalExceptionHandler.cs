using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.Exceptions;

/// <summary>
/// Global exception handler implementing IExceptionHandler for minimal APIs.
/// Catches all unhandled exceptions and returns RFC 7807 ProblemDetails responses.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var correlationId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "Unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}",
            correlationId,
            httpContext.Request.Path,
            httpContext.Request.Method);

        var problemDetails = CreateProblemDetails(httpContext, exception, correlationId);

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static Microsoft.AspNetCore.Mvc.ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        Exception exception,
        string correlationId)
    {
        var (statusCode, title, type) = MapExceptionToStatusCode(exception);

        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = GetExceptionDetail(exception, statusCode),
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["correlationId"] = correlationId;
        problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = new[]
            {
                new { field = "validation", message = validationException.Message }
            };
        }

        if (exception is ArgumentException argumentException)
        {
            problemDetails.Extensions["errors"] = new[]
            {
                new { field = argumentException.ParamName ?? "unknown", message = argumentException.Message }
            };
        }

        return problemDetails;
    }

    private static (int StatusCode, string Title, string Type) MapExceptionToStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "https://tools.ietf.org/html/rfc7231#section-6.5.1"),

            ArgumentNullException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://tools.ietf.org/html/rfc7231#section-6.5.1"),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://tools.ietf.org/html/rfc7231#section-6.5.1"),

            AuthenticationException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "https://tools.ietf.org/html/rfc7235#section-3.1"),

            UnauthorizedAccessException => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "https://tools.ietf.org/html/rfc7231#section-6.5.3"),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                "https://tools.ietf.org/html/rfc7231#section-6.5.4"),

            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "https://tools.ietf.org/html/rfc7231#section-6.5.8"),

            OperationCanceledException => (
                StatusCodes.Status499ClientClosedRequest,
                "Client Closed Request",
                "https://httpstatuses.com/499"),

            NotImplementedException => (
                StatusCodes.Status501NotImplemented,
                "Not Implemented",
                "https://tools.ietf.org/html/rfc7231#section-6.6.2"),

            TimeoutException => (
                StatusCodes.Status504GatewayTimeout,
                "Gateway Timeout",
                "https://tools.ietf.org/html/rfc7231#section-6.6.5"),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "https://tools.ietf.org/html/rfc7231#section-6.6.1")
        };
    }

    private static string GetExceptionDetail(Exception exception, int statusCode)
    {
        // Only expose exception details for client errors in development
        // For server errors, always return a generic message
        if (statusCode >= 500)
        {
            return "An unexpected error occurred. Please try again later.";
        }

        return exception.Message;
    }
}
