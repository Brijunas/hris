namespace BuildingBlocks.Security.Masking;

/// <summary>
/// Service for masking sensitive data in API responses.
/// Provides consistent masking patterns for PII (Personally Identifiable Information).
/// </summary>
public interface IDataMasker
{
    /// <summary>
    /// Masks a national ID (SSN) showing only the last 4 digits.
    /// </summary>
    /// <param name="nationalId">The national ID to mask (e.g., "123-45-6789").</param>
    /// <returns>The masked ID (e.g., "***-**-6789"), or empty string if input is null/empty.</returns>
    string MaskNationalId(string? nationalId);

    /// <summary>
    /// Masks a phone number showing only the last 4 digits.
    /// </summary>
    /// <param name="phone">The phone number to mask (e.g., "555-123-4567").</param>
    /// <returns>The masked phone (e.g., "***-***-4567"), or empty string if input is null/empty.</returns>
    string MaskPhone(string? phone);

    /// <summary>
    /// Masks an email address showing only the first character and domain.
    /// </summary>
    /// <param name="email">The email address to mask (e.g., "john.doe@example.com").</param>
    /// <returns>The masked email (e.g., "j***@example.com"), or empty string if input is null/empty.</returns>
    string MaskEmail(string? email);
}
