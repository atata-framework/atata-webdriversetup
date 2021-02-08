namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a method that resolves the driver latest version.
    /// </summary>
    public interface IGetsDriverLatestVersion
    {
        /// <summary>
        /// Gets the driver latest version.
        /// </summary>
        /// <returns>The latest version string.</returns>
        string GetDriverLatestVersion();
    }
}
