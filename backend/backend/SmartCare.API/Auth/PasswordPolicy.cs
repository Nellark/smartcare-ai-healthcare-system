using System.Text.RegularExpressions;

namespace SmartCare.API.Auth;

/// <summary>
/// Enforces strong password policy:
/// - Minimum 8 characters
/// - At least one uppercase letter
/// - At least one lowercase letter
/// - At least one digit
/// - At least one special character (!@#$%^&amp;*...)
/// </summary>
public static class PasswordPolicy
{
    private static readonly Regex UppercaseRegex = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex LowercaseRegex = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex DigitRegex = new(@"\d", RegexOptions.Compiled);
    private static readonly Regex SpecialCharRegex = new(@"[!@#$%^&*()_+\-=\[\]{}|;':"",./<>?\\]", RegexOptions.Compiled);

    public static PasswordValidationResult Validate(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required.");
            return new PasswordValidationResult(false, errors);
        }

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long.");

        if (password.Length > 128)
            errors.Add("Password must not exceed 128 characters.");

        if (!UppercaseRegex.IsMatch(password))
            errors.Add("Password must contain at least one uppercase letter.");

        if (!LowercaseRegex.IsMatch(password))
            errors.Add("Password must contain at least one lowercase letter.");

        if (!DigitRegex.IsMatch(password))
            errors.Add("Password must contain at least one digit.");

        if (!SpecialCharRegex.IsMatch(password))
            errors.Add("Password must contain at least one special character (e.g. !@#$%^&*).");

        return new PasswordValidationResult(errors.Count == 0, errors);
    }
}

public record PasswordValidationResult(bool IsValid, List<string> Errors);
