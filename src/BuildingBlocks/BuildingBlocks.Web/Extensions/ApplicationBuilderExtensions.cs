using BuildingBlocks.Web.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Web.Extensions;

/// <summary>
/// Extension methods for configuring BuildingBlocks.Web middleware pipeline.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds all BuildingBlocks.Web middleware to the application pipeline.
    /// Should be called after UseAuthentication and UseAuthorization.
    /// </summary>
    public static IApplicationBuilder UseBuildingBlocksWeb(this IApplicationBuilder app)
    {
        // Add global exception handling
        app.UseExceptionHandler();

        // Add user context middleware
        app.UseMiddleware<UserContextMiddleware>();

        return app;
    }

    /// <summary>
    /// Adds only the user context middleware to the pipeline.
    /// Use when you want fine-grained control over middleware ordering.
    /// </summary>
    public static IApplicationBuilder UseUserContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserContextMiddleware>();
    }
}
