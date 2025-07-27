namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a method that resolves the driver closest version.
/// </summary>
public interface IGetsDriverClosestVersion
{
    /// <summary>
    /// Gets the driver version closest to <paramref name="version"/>.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="platform">The target OS platform.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with the driver closest version.</returns>
    Task<string> GetDriverClosestVersionAsync(
        string version,
        TargetOSPlatform platform,
        CancellationToken cancellationToken = default);
}
