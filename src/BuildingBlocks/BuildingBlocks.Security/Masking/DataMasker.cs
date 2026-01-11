using System.Text.RegularExpressions;

namespace BuildingBlocks.Security.Masking;

/// <summary>
/// Implementation of <see cref="IDataMasker"/> for consistent PII masking.
/// </summary>
public sealed partial class DataMasker : IDataMasker
{
    // Regex to extract digits from phone/national ID
    [GeneratedRegex(@"\d")]
    private static partial Regex DigitRegex();

    /// <inheritdoc />
    public string MaskNationalId(string? nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
        {
            return string.Empty;
        }

        // Extract only digits
        var digits = DigitRegex().Matches(nationalId)
            .Select(m => m.Value)
            .ToArray();

        if (digits.Length < 4)
        {
            // Not enough digits to mask meaningfully, mask everything
            return "***-**-****";
        }

        // Get last 4 digits
        var lastFour = string.Join("", digits.TakeLast(4));

        // Return in SSN format with masked prefix
        return $"***-**-{lastFour}";
    }

    /// <inheritdoc />
    public string MaskPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return string.Empty;
        }

        // Extract only digits
        var digits = DigitRegex().Matches(phone)
            .Select(m => m.Value)
            .ToArray();

        if (digits.Length < 4)
        {
            // Not enough digits to mask meaningfully
            return "***-***-****";
        }

        // Get last 4 digits
        var lastFour = string.Join("", digits.TakeLast(4));

        // Return in phone format with masked prefix
        return $"***-***-{lastFour}";
    }

    /// <inheritdoc />
    public string MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return string.Empty;
        }

        var atIndex = email.IndexOf('@');
        if (atIndex < 1)
        {
            // Invalid email format or @ at start
            return "***@***";
        }

        var localPart = email[..atIndex];
        var domain = email[(atIndex + 1)..];

        // Show first character of local part, mask the rest
        var firstChar = localPart[0];

        return $"{firstChar}***@{domain}";
    }
}
