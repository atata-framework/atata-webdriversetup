namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a set of utility methods for version strings.
/// </summary>
public static class VersionUtils
{
    /// <summary>
    /// Trims the revision number of the version.
    /// </summary>
    /// <example>
    /// <c>"1.2.3.4"</c> -> <c>"1.2.3"</c>.
    /// </example>
    /// <param name="version">The version.</param>
    /// <returns>The trimmed version string.</returns>
    public static string TrimRevision(string version) =>
        CutVersion(version, 3);

    /// <summary>
    /// Trims the patch and revision numbers of the version.
    /// </summary>
    /// <example>
    /// <c>"1.2.3.4"</c> -> <c>"1.2"</c>.
    /// </example>
    /// <param name="version">The version.</param>
    /// <returns>The trimmed version string.</returns>
    public static string TrimPatch(string version) =>
        CutVersion(version, 2);

    /// <summary>
    /// Trims the minor, patch and revision numbers of the version.
    /// </summary>
    /// <example>
    /// <c>"1.2.3.4"</c> -> <c>"1"</c>.
    /// </example>
    /// <param name="version">The version.</param>
    /// <returns>The trimmed version string.</returns>
    public static string TrimMinor(string version) =>
        CutVersion(version, 1);

    /// <summary>
    /// Gets the major number.
    /// </summary>
    /// <example>
    /// <c>"1.2.3.4"</c> -> <c>1</c>.
    /// </example>
    /// <param name="version">The version.</param>
    /// <returns>The major version number.</returns>
    public static int GetMajorNumber(string version) =>
        int.Parse(TrimMinor(version));

    internal static Version Parse(string version)
    {
        var parts = GetParts(version, 4);

        return new Version(
            int.Parse(parts[0]),
            int.Parse(parts[1]),
            int.Parse(parts[2]),
            int.Parse(parts[3]));
    }

    private static string CutVersion(string version, int versionNumbersToLeave)
    {
        var parts = GetParts(version, versionNumbersToLeave);

        return string.Join(".", parts.Take(versionNumbersToLeave));
    }

    private static string[] GetParts(string version, int count)
    {
        Guard.ThrowIfNullOrWhitespace(version);

        string[] parts = version.Split('.');

        if (parts.Length < count)
            throw new ArgumentException($"""Inappropriate "{version}" {nameof(version)} format. Version should consist at least of {count} numbers.""", nameof(version));

        return parts;
    }

    /// <summary>
    /// Gets the version numbers count.
    /// </summary>
    /// <example>
    /// <c>"1.2.3.4"</c> -> 4.
    /// </example>
    /// <param name="version">The version.</param>
    /// <returns>The count of numbers.</returns>
    public static int GetNumbersCount(string version)
    {
        Guard.ThrowIfNullOrWhitespace(version);

        return version.Count(x => x == '.') + 1;
    }
}
