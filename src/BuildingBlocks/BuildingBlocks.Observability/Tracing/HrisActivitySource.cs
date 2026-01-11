using System.Diagnostics;

namespace BuildingBlocks.Observability.Tracing;

/// <summary>
/// Provides a centralized ActivitySource for creating custom spans in the HRIS application.
/// </summary>
public static class HrisActivitySource
{
    /// <summary>
    /// The name of the activity source.
    /// </summary>
    public const string Name = "Hris.Application";

    /// <summary>
    /// The version of the activity source.
    /// </summary>
    public const string Version = "1.0.0";

    /// <summary>
    /// The shared ActivitySource instance for the HRIS application.
    /// </summary>
    public static readonly ActivitySource Source = new(Name, Version);

    /// <summary>
    /// Starts a new activity with the specified name.
    /// </summary>
    /// <param name="name">The name of the activity.</param>
    /// <param name="kind">The kind of activity.</param>
    /// <returns>The started activity, or null if no listeners are registered.</returns>
    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        return Source.StartActivity(name, kind);
    }

    /// <summary>
    /// Starts a new activity for a domain operation.
    /// </summary>
    /// <param name="operationName">The name of the domain operation.</param>
    /// <returns>The started activity, or null if no listeners are registered.</returns>
    public static Activity? StartDomainActivity(string operationName)
    {
        return Source.StartActivity($"Domain.{operationName}", ActivityKind.Internal);
    }

    /// <summary>
    /// Starts a new activity for a command handler.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <returns>The started activity, or null if no listeners are registered.</returns>
    public static Activity? StartCommandActivity(string commandName)
    {
        return Source.StartActivity($"Command.{commandName}", ActivityKind.Internal);
    }

    /// <summary>
    /// Starts a new activity for a query handler.
    /// </summary>
    /// <param name="queryName">The name of the query.</param>
    /// <returns>The started activity, or null if no listeners are registered.</returns>
    public static Activity? StartQueryActivity(string queryName)
    {
        return Source.StartActivity($"Query.{queryName}", ActivityKind.Internal);
    }

    /// <summary>
    /// Starts a new activity for an event handler.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <returns>The started activity, or null if no listeners are registered.</returns>
    public static Activity? StartEventActivity(string eventName)
    {
        return Source.StartActivity($"Event.{eventName}", ActivityKind.Consumer);
    }

    /// <summary>
    /// Starts a new activity for a background job.
    /// </summary>
    /// <param name="jobName">The name of the job.</param>
    /// <returns>The started activity, or null if no listeners are registered.</returns>
    public static Activity? StartJobActivity(string jobName)
    {
        return Source.StartActivity($"Job.{jobName}", ActivityKind.Internal);
    }
}
