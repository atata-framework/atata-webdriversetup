namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a method that resolves the driver latest version.
/// </summary>
public interface IGetsDriverLatestVersion
{
    /// <summary>
    /// Gets the driver latest version.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with the latest version string.</returns>
    Task<string> GetDriverLatestVersionAsync(CancellationToken cancellationToken = default);
}
