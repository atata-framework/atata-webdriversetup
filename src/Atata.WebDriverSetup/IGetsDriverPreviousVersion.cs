namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a method that tries to resolves the driver latest version.
/// </summary>
public interface IGetsDriverPreviousVersion
{
    /// <summary>
    /// Tries to get the driver version previous to <paramref name="version"/>.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="architecture">The architecture.</param>
    /// <param name="previousVersion">The previous version.</param>
    /// <returns><see langword="true"/> if the previous version is found; otherwise, <see langword="false"/>.</returns>
    bool TryGetDriverPreviousVersion(string version, Architecture architecture, out string previousVersion);
}
