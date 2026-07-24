namespace EHRPlatform.Common.Utilities;

/// <summary>
/// Lightweight argument validation helpers used throughout the Common library.
/// Centralises null/empty checks to avoid duplication across files.
/// </summary>
internal static class ArgumentGuard
{
    public static void NotNull<T>(T? argument, string parameterName) where T : class
    {
        if (argument == null)
            throw new ArgumentNullException(parameterName);
    }

    public static void NotNullOrEmpty(string? argument, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException("Value cannot be null or empty.", parameterName);
    }
}
