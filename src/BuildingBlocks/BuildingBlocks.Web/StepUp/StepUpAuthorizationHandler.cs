using BuildingBlocks.Persistence.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.StepUp;

/// <summary>
/// Authorization handler that validates step-up authentication is active.
/// Checks the IUserContext.IsStepUpActive property.
/// </summary>
public sealed class StepUpAuthorizationHandler : AuthorizationHandler<StepUpRequirement>
{
    private readonly IUserContext _userContext;
    private readonly ILogger<StepUpAuthorizationHandler> _logger;

    public StepUpAuthorizationHandler(
        IUserContext userContext,
        ILogger<StepUpAuthorizationHandler> logger)
    {
        _userContext = userContext;
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        StepUpRequirement requirement)
    {
        if (_userContext.UserId == Guid.Empty)
        {
            _logger.LogWarning("Step-up authorization failed: No user context available");
            context.Fail(new AuthorizationFailureReason(this, "User context not available"));
            return Task.CompletedTask;
        }

        if (!_userContext.IsStepUpActive)
        {
            _logger.LogWarning(
                "Step-up authorization failed for user {UserId}: Step-up authentication required",
                _userContext.UserId);
            context.Fail(new AuthorizationFailureReason(this, "Step-up authentication required"));
            return Task.CompletedTask;
        }

        _logger.LogDebug("Step-up authorization succeeded for user {UserId}", _userContext.UserId);
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization requirement for step-up authentication.
/// </summary>
public sealed class StepUpRequirement : IAuthorizationRequirement
{
}
