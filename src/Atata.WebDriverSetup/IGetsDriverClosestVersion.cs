namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a method that tries to resolve the driver closest version.
/// </summary>
public interface IGetsDriverClosestVersion
{
    /// <summary>
    /// Tries to get the driver version closest to <paramref name="version"/>.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="architecture">The architecture.</param>
    /// <param name="closestVersion">The closest version.</param>
    /// <returns><see langword="true"/> if the closest version is found; otherwise, <see langword="false"/>.</returns>
    bool TryGetDriverClosestVersion(string version, Architecture architecture, out string closestVersion);
}
