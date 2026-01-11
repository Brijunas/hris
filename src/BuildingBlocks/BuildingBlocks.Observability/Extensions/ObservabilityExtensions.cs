using BuildingBlocks.Observability.Correlation;
using BuildingBlocks.Observability.Metrics;
using BuildingBlocks.Observability.Tracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Observability.Extensions;

/// <summary>
/// Extension methods for configuring observability in the HRIS application.
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Adds HRIS observability services including OpenTelemetry tracing and metrics.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceName">The name of the service for OpenTelemetry resource.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddHrisObservability(this IServiceCollection services, string serviceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        // Register correlation ID services
        services.AddSingleton<ICorrelationIdAccessor, CorrelationIdAccessor>();

        // Register metrics
        services.AddSingleton<HrisMetrics>();

        // Configure OpenTelemetry
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: serviceName,
                    serviceVersion: typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString() ?? "1.0.0")
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    ["service.instance.id"] = Environment.MachineName
                }))
            .WithTracing(tracing => tracing
                .AddSource(HrisActivitySource.Name)
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Filter = httpContext =>
                    {
                        // Filter out health check and metrics endpoints
                        var path = httpContext.Request.Path.Value;
                        return path is null ||
                               (!path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) &&
                                !path.StartsWith("/metrics", StringComparison.OrdinalIgnoreCase) &&
                                !path.StartsWith("/ready", StringComparison.OrdinalIgnoreCase) &&
                                !path.StartsWith("/live", StringComparison.OrdinalIgnoreCase));
                    };
                    options.EnrichWithHttpRequest = (activity, request) =>
                    {
                        activity.SetTag("http.request.header.host", request.Host.Value);
                        if (request.HttpContext.Items.TryGetValue(CorrelationIdMiddleware.CorrelationIdItemKey, out var correlationId))
                        {
                            activity.SetTag(CorrelationIdMiddleware.CorrelationIdActivityTag, correlationId);
                        }
                    };
                    options.EnrichWithHttpResponse = (activity, response) =>
                    {
                        activity.SetTag("http.response.status_code", response.StatusCode);
                    };
                    options.EnrichWithException = (activity, exception) =>
                    {
                        activity.SetTag("exception.type", exception.GetType().FullName);
                        activity.SetTag("exception.message", exception.Message);
                    };
                })
                .AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpRequestMessage = (activity, request) =>
                    {
                        activity.SetTag("http.request.uri", request.RequestUri?.ToString());
                    };
                    options.EnrichWithHttpResponseMessage = (activity, response) =>
                    {
                        activity.SetTag("http.response.status_code", (int)response.StatusCode);
                    };
                    options.EnrichWithException = (activity, exception) =>
                    {
                        activity.SetTag("exception.type", exception.GetType().FullName);
                        activity.SetTag("exception.message", exception.Message);
                    };
                })
                .AddEntityFrameworkCoreInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.SetDbStatementForStoredProcedure = true;
                    options.EnrichWithIDbCommand = (activity, command) =>
                    {
                        activity.SetTag("db.statement.type", command.CommandType.ToString());
                    };
                })
                .AddOtlpExporter())
            .WithMetrics(metrics => metrics
                .AddMeter(HrisMetrics.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter()
                .AddPrometheusExporter());

        return services;
    }

    /// <summary>
    /// Adds HRIS observability services with custom configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceName">The name of the service for OpenTelemetry resource.</param>
    /// <param name="configureTracing">Optional action to configure tracing.</param>
    /// <param name="configureMetrics">Optional action to configure metrics.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddHrisObservability(
        this IServiceCollection services,
        string serviceName,
        Action<TracerProviderBuilder>? configureTracing,
        Action<MeterProviderBuilder>? configureMetrics)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        // Register correlation ID services
        services.AddSingleton<ICorrelationIdAccessor, CorrelationIdAccessor>();

        // Register metrics
        services.AddSingleton<HrisMetrics>();

        // Configure OpenTelemetry
        var otel = services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: serviceName,
                    serviceVersion: typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString() ?? "1.0.0")
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    ["service.instance.id"] = Environment.MachineName
                }));

        if (configureTracing is not null)
        {
            otel.WithTracing(configureTracing);
        }

        if (configureMetrics is not null)
        {
            otel.WithMetrics(configureMetrics);
        }

        return services;
    }

    /// <summary>
    /// Uses the correlation ID middleware in the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }

    /// <summary>
    /// Maps the Prometheus metrics endpoint.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <param name="path">The path for the metrics endpoint. Defaults to "/metrics".</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication UseHrisPrometheusMetrics(this WebApplication app, string path = "/metrics")
    {
        app.UseOpenTelemetryPrometheusScrapingEndpoint(path);
        return app;
    }
}
