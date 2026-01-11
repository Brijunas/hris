using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Observability.Correlation;

/// <summary>
/// ASP.NET Core middleware that manages correlation IDs for request tracing.
/// Reads the correlation ID from the X-Correlation-ID header or generates a new one,
/// stores it in HttpContext.Items, and adds it to the response headers.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    /// <summary>
    /// The header name used for correlation ID propagation.
    /// </summary>
    public const string CorrelationIdHeader = "X-Correlation-ID";

    /// <summary>
    /// The key used to store correlation ID in HttpContext.Items.
    /// </summary>
    public const string CorrelationIdItemKey = "CorrelationId";

    /// <summary>
    /// The tag name used for Activity correlation ID.
    /// </summary>
    public const string CorrelationIdActivityTag = "correlation.id";

    private readonly RequestDelegate _next;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="correlationIdAccessor">The correlation ID accessor.</param>
    public CorrelationIdMiddleware(RequestDelegate next, ICorrelationIdAccessor correlationIdAccessor)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _correlationIdAccessor = correlationIdAccessor ?? throw new ArgumentNullException(nameof(correlationIdAccessor));
    }

    /// <summary>
    /// Processes the HTTP request and manages correlation ID.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        // Store in HttpContext.Items
        context.Items[CorrelationIdItemKey] = correlationId;

        // Store in AsyncLocal accessor for cross-cutting concerns
        _correlationIdAccessor.SetCorrelationId(correlationId);

        // Add to current Activity as a tag
        Activity.Current?.AddTag(CorrelationIdActivityTag, correlationId);

        // Add to response headers (before response starts)
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
            {
                context.Response.Headers.Append(CorrelationIdHeader, correlationId);
            }
            return Task.CompletedTask;
        });

        await _next(context);
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        // Try to get from request header
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue) &&
            !string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.ToString();
        }

        // Generate new correlation ID
        return Guid.NewGuid().ToString("D");
    }
}
