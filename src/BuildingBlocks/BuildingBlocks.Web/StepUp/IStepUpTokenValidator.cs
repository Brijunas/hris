namespace BuildingBlocks.Web.StepUp;

/// <summary>
/// Interface for validating step-up authentication tokens.
/// Implementation should be provided by Infrastructure layer (e.g., Redis-based).
/// </summary>
public interface IStepUpTokenValidator
{
    /// <summary>
    /// Validates that the step-up token is valid for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier to validate the token against.</param>
    /// <param name="token">The step-up token to validate.</param>
    /// <returns>True if the token is valid and not expired; otherwise, false.</returns>
    Task<bool> ValidateAsync(Guid userId, string token);
}
