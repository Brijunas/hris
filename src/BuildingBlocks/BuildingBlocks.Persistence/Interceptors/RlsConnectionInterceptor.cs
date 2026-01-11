using System.Data.Common;
using BuildingBlocks.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Persistence.Interceptors;

/// <summary>
/// EF Core DbConnectionInterceptor that sets PostgreSQL session variables for Row-Level Security (RLS).
/// This interceptor ensures that every database connection has the appropriate user context
/// for RLS policies to function correctly.
/// </summary>
public sealed class RlsConnectionInterceptor : DbConnectionInterceptor
{
    private readonly IUserContext _userContext;

    public RlsConnectionInterceptor(IUserContext userContext)
    {
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetRlsVariables(connection);
        base.ConnectionOpened(connection, eventData);
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await SetRlsVariablesAsync(connection, cancellationToken);
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    private void SetRlsVariables(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = BuildSetLocalCommand();
        command.ExecuteNonQuery();
    }

    private async Task SetRlsVariablesAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = BuildSetLocalCommand();
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private string BuildSetLocalCommand()
    {
        var userId = EscapePostgresValue(_userContext.UserId.ToString());
        var orgUnitId = EscapePostgresValue(_userContext.OrgUnitId.ToString());
        var stepUp = _userContext.IsStepUpActive ? "true" : "false";

        return $"""
            SET LOCAL app.user_id = '{userId}';
            SET LOCAL app.org_unit_id = '{orgUnitId}';
            SET LOCAL app.step_up = '{stepUp}';
            """;
    }

    /// <summary>
    /// Escapes single quotes in PostgreSQL string values to prevent SQL injection.
    /// </summary>
    private static string EscapePostgresValue(string value)
    {
        return value.Replace("'", "''");
    }
}
