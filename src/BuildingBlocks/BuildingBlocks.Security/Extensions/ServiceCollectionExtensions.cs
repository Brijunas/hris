using BuildingBlocks.Security.Hashing;
using BuildingBlocks.Security.Masking;
using BuildingBlocks.Security.Password;
using BuildingBlocks.Security.Serialization;
using BuildingBlocks.Security.Signing;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Security.Extensions;

/// <summary>
/// Extension methods for registering security services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all BuildingBlocks.Security services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        services.AddSingleton<IHashChainService, HashChainService>();
        services.AddSingleton<ISigningService, SigningService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IDataMasker, DataMasker>();
        services.AddSingleton<ICanonicalJsonSerializer, CanonicalJsonSerializer>();

        return services;
    }

    /// <summary>
    /// Adds hash chain services for tamper-evident audit trails.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddHashChainServices(this IServiceCollection services)
    {
        services.AddSingleton<IHashChainService, HashChainService>();
        services.AddSingleton<ICanonicalJsonSerializer, CanonicalJsonSerializer>();

        return services;
    }

    /// <summary>
    /// Adds cryptographic signing services for audit checkpoints.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSigningServices(this IServiceCollection services)
    {
        services.AddSingleton<ISigningService, SigningService>();

        return services;
    }

    /// <summary>
    /// Adds password hashing services using Argon2id.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPasswordHashingServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }

    /// <summary>
    /// Adds data masking services for PII protection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDataMaskingServices(this IServiceCollection services)
    {
        services.AddSingleton<IDataMasker, DataMasker>();

        return services;
    }
}
