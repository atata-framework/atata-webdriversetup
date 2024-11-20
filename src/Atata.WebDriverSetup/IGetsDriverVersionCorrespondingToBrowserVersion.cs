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
    /// <returns>The driver version string.</returns>
    string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion, TargetOSPlatform platform);
}
