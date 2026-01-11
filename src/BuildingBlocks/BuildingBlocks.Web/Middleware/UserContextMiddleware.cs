using System.Security.Claims;
using BuildingBlocks.Web.Context;
using BuildingBlocks.Web.StepUp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.Middleware;

/// <summary>
/// Middleware that populates UserContext from JWT claims and step-up token headers/cookies.
/// Extracts user_id, org_unit_id from JWT claims and validates step-up tokens.
/// </summary>
public sealed class UserContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserContextMiddleware> _logger;

    /// <summary>
    /// JWT claim type for user identifier.
    /// </summary>
    public const string UserIdClaimType = "user_id";

    /// <summary>
    /// JWT claim type for organizational unit identifier.
    /// </summary>
    public const string OrgUnitIdClaimType = "org_unit_id";

    /// <summary>
    /// Header name for step-up token.
    /// </summary>
    public const string StepUpTokenHeader = "X-StepUp-Token";

    /// <summary>
    /// Cookie name for step-up token.
    /// </summary>
    public const string StepUpTokenCookie = "stepup_token";

    public UserContextMiddleware(RequestDelegate next, ILogger<UserContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        UserContext userContext,
        IStepUpTokenValidator? stepUpValidator = null)
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            PopulateFromClaims(httpContext.User, userContext);
            await ValidateStepUpToken(httpContext, userContext, stepUpValidator);

            _logger.LogDebug(
                "UserContext populated: UserId={UserId}, OrgUnitId={OrgUnitId}, StepUpActive={StepUpActive}",
                userContext.UserId,
                userContext.OrgUnitId,
                userContext.IsStepUpActive);
        }

        await _next(httpContext);
    }

    private void PopulateFromClaims(ClaimsPrincipal user, UserContext userContext)
    {
        // Extract user_id claim
        var userIdClaim = user.FindFirst(UserIdClaimType)
            ?? user.FindFirst(ClaimTypes.NameIdentifier)
            ?? user.FindFirst("sub");

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            userContext.UserId = userId;
        }
        else
        {
            _logger.LogWarning("Could not extract valid UserId from claims");
        }

        // Extract org_unit_id claim
        var orgUnitIdClaim = user.FindFirst(OrgUnitIdClaimType);
        if (orgUnitIdClaim != null && Guid.TryParse(orgUnitIdClaim.Value, out var orgUnitId))
        {
            userContext.OrgUnitId = orgUnitId;
        }
        else
        {
            _logger.LogDebug("OrgUnitId claim not found or invalid");
        }
    }

    private async Task ValidateStepUpToken(
        HttpContext httpContext,
        UserContext userContext,
        IStepUpTokenValidator? stepUpValidator)
    {
        if (stepUpValidator == null)
        {
            return;
        }

        // Try to get step-up token from header first, then from cookie
        var stepUpToken = GetStepUpToken(httpContext);
        if (string.IsNullOrWhiteSpace(stepUpToken))
        {
            return;
        }

        if (userContext.UserId == Guid.Empty)
        {
            _logger.LogWarning("Cannot validate step-up token without valid UserId");
            return;
        }

        try
        {
            userContext.IsStepUpActive = await stepUpValidator.ValidateAsync(userContext.UserId, stepUpToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating step-up token for user {UserId}", userContext.UserId);
            userContext.IsStepUpActive = false;
        }
    }

    private static string? GetStepUpToken(HttpContext httpContext)
    {
        // Check header first
        if (httpContext.Request.Headers.TryGetValue(StepUpTokenHeader, out var headerValue)
            && !string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.ToString();
        }

        // Fall back to cookie
        if (httpContext.Request.Cookies.TryGetValue(StepUpTokenCookie, out var cookieValue)
            && !string.IsNullOrWhiteSpace(cookieValue))
        {
            return cookieValue;
        }

        return null;
    }
}
