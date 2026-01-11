using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Web.StepUp;

/// <summary>
/// Authorization attribute that requires step-up authentication to be active.
/// Apply to endpoints that require elevated security (e.g., PII reveal operations).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class StepUpRequiredAttribute : AuthorizeAttribute
{
    /// <summary>
    /// The policy name for step-up authorization.
    /// </summary>
    public const string PolicyName = "StepUpRequired";

    public StepUpRequiredAttribute()
        : base(PolicyName)
    {
    }
}
