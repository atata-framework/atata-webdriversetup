namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a method that resolves the installed browser version.
    /// </summary>
    public interface IGetsInstalledBrowserVersion
    {
        /// <summary>
        /// Gets the installed browser version.
        /// </summary>
        /// <returns>The version number.</returns>
        string GetInstalledBrowserVersion();
    }
}
