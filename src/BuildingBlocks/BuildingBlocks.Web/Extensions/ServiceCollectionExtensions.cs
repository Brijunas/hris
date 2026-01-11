using BuildingBlocks.Persistence.Abstractions;
using BuildingBlocks.Web.Context;
using BuildingBlocks.Web.Exceptions;
using BuildingBlocks.Web.StepUp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Extensions;

/// <summary>
/// Extension methods for configuring BuildingBlocks.Web services in DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all BuildingBlocks.Web services to the service collection.
    /// </summary>
    public static IServiceCollection AddBuildingBlocksWeb(this IServiceCollection services)
    {
        // Register global exception handler
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // Register UserContext as scoped (per-request)
        services.AddScoped<UserContext>();
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<UserContext>());

        // Register step-up authorization
        services.AddScoped<IAuthorizationHandler, StepUpAuthorizationHandler>();

        return services;
    }

    /// <summary>
    /// Adds step-up authentication policy to authorization options.
    /// </summary>
    public static IServiceCollection AddStepUpAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(StepUpRequiredAttribute.PolicyName, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new StepUpRequirement());
            });

        return services;
    }

    /// <summary>
    /// Adds step-up token validator implementation.
    /// Use this to register a custom IStepUpTokenValidator.
    /// </summary>
    public static IServiceCollection AddStepUpTokenValidator<TValidator>(this IServiceCollection services)
        where TValidator : class, IStepUpTokenValidator
    {
        services.AddScoped<IStepUpTokenValidator, TValidator>();
        return services;
    }

    /// <summary>
    /// Adds step-up token validator implementation with a factory.
    /// </summary>
    public static IServiceCollection AddStepUpTokenValidator(
        this IServiceCollection services,
        Func<IServiceProvider, IStepUpTokenValidator> factory)
    {
        services.AddScoped(factory);
        return services;
    }
}
