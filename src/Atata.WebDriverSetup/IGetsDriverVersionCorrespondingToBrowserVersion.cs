namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a method that resolves the driver version corresponding to the browser version.
/// </summary>
public interface IGetsDriverVersionCorrespondingToBrowserVersion
{
    /// <summary>
    /// Gets the driver version corresponding to the browser version.
    /// </summary>
    /// <param name="browserVersion">The browser version.</param>
    /// <param name="platform">The target OS platform.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with the driver version string.</returns>
    Task<string> GetDriverVersionCorrespondingToBrowserVersionAsync(
        string browserVersion,
        TargetOSPlatform platform,
        CancellationToken cancellationToken = default);
}
