namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the strategy of driver setup.
/// </summary>
public interface IDriverSetupStrategy
{
    /// <summary>
    /// Gets the name of the driver binary file.
    /// </summary>
    /// <param name="platform">The target OS platform.</param>
    /// <returns>The file name.</returns>
    string GetDriverBinaryFileName(TargetOSPlatform platform);

    /// <summary>
    /// Gets the driver download URL.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="platform">The target OS platform.</param>
    /// <returns>The driver download URL.</returns>
    Uri GetDriverDownloadUrl(string version, TargetOSPlatform platform);
}
