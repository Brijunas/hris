using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Endpoints;

/// <summary>
/// Extension methods for configuring minimal API endpoint conventions.
/// Provides consistent configuration across all API endpoints.
/// </summary>
public static class ApiEndpointConventions
{
    /// <summary>
    /// Applies standard API conventions to an endpoint.
    /// Includes OpenAPI metadata, produces declarations, and error responses.
    /// </summary>
    public static RouteHandlerBuilder WithApiConventions(this RouteHandlerBuilder builder)
    {
        return builder
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Applies conventions for an endpoint that returns a single resource.
    /// </summary>
    public static RouteHandlerBuilder WithGetConventions<TResponse>(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces<TResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithApiConventions();
    }

    /// <summary>
    /// Applies conventions for an endpoint that returns a list of resources.
    /// </summary>
    public static RouteHandlerBuilder WithListConventions<TResponse>(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces<TResponse>(StatusCodes.Status200OK)
            .WithApiConventions();
    }

    /// <summary>
    /// Applies conventions for an endpoint that creates a resource.
    /// </summary>
    public static RouteHandlerBuilder WithCreateConventions<TResponse>(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces<TResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithApiConventions();
    }

    /// <summary>
    /// Applies conventions for an endpoint that creates a resource without returning the created object.
    /// </summary>
    public static RouteHandlerBuilder WithCreateConventions(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithApiConventions();
    }

    /// <summary>
    /// Applies conventions for an endpoint that updates a resource.
    /// </summary>
    public static RouteHandlerBuilder WithUpdateConventions<TResponse>(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces<TResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithApiConventions();
    }

    /// <summary>
    /// Applies conventions for an endpoint that updates a resource without returning content.
    /// </summary>
    public static RouteHandlerBuilder WithUpdateConventions(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithApiConventions();
    }

    /// <summary>
    /// Applies conventions for an endpoint that deletes a resource.
    /// </summary>
    public static RouteHandlerBuilder WithDeleteConventions(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithApiConventions();
    }

    /// <summary>
    /// Applies conventions for an endpoint that requires step-up authentication.
    /// </summary>
    public static RouteHandlerBuilder WithStepUpConventions(this RouteHandlerBuilder builder)
    {
        return builder
            .RequireAuthorization(StepUp.StepUpRequiredAttribute.PolicyName);
    }

    /// <summary>
    /// Applies conventions for an endpoint that reveals PII data (requires step-up).
    /// </summary>
    public static RouteHandlerBuilder WithPiiRevealConventions<TResponse>(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces<TResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithStepUpConventions()
            .WithApiConventions();
    }

    /// <summary>
    /// Adds versioning metadata to an endpoint.
    /// </summary>
    public static RouteHandlerBuilder WithApiVersion(this RouteHandlerBuilder builder, int version)
    {
        return builder.WithMetadata(new ApiVersionMetadata(version));
    }

    /// <summary>
    /// Adds deprecation metadata to an endpoint.
    /// </summary>
    public static RouteHandlerBuilder AsDeprecated(this RouteHandlerBuilder builder, string? message = null)
    {
        return builder.WithMetadata(new DeprecatedEndpointMetadata(message));
    }

    /// <summary>
    /// Adds summary and description to an endpoint for OpenAPI documentation.
    /// </summary>
    public static RouteHandlerBuilder WithDescription(
        this RouteHandlerBuilder builder,
        string summary,
        string? description = null)
    {
        builder.WithSummary(summary);
        if (!string.IsNullOrEmpty(description))
        {
            builder.WithDescription(description);
        }
        return builder;
    }

    /// <summary>
    /// Groups endpoints by feature/module.
    /// </summary>
    public static RouteHandlerBuilder WithFeatureTag(this RouteHandlerBuilder builder, string featureName)
    {
        return builder.WithTags(featureName);
    }

    /// <summary>
    /// Adds rate limiting metadata to an endpoint.
    /// </summary>
    public static RouteHandlerBuilder WithRateLimiting(this RouteHandlerBuilder builder, string policyName)
    {
        return builder.RequireRateLimiting(policyName);
    }

    /// <summary>
    /// Disables anti-forgery validation for an endpoint.
    /// Use with caution, only for specific scenarios like webhooks.
    /// </summary>
    public static RouteHandlerBuilder DisableAntiforgery(this RouteHandlerBuilder builder)
    {
        return builder.DisableAntiforgery();
    }
}

/// <summary>
/// Metadata for API versioning.
/// </summary>
public sealed record ApiVersionMetadata(int Version);

/// <summary>
/// Metadata indicating an endpoint is deprecated.
/// </summary>
public sealed record DeprecatedEndpointMetadata(string? Message);
