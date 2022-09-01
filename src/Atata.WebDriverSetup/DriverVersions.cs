using System;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a set of predefined version values and a functionality for driver version management.
    /// </summary>
    public static class DriverVersions
    {
        /// <summary>
        /// The browser version prefix.
        /// </summary>
        public const string BrowserVersionPrefix = "browser:";

        /// <summary>
        /// The automatic version detection.
        /// If the version cannot be detected automatically, latest driver version should be used.
        /// </summary>
        public const string Auto = "auto";

        /// <summary>
        /// The latest version.
        /// </summary>
        public const string Latest = "latest";

        /// <summary>
        /// Creates the string for version that corresponds to the specific browser version.
        /// </summary>
        /// <param name="browserVersion">The browser version.</param>
        /// <returns>The version string.</returns>
        public static string CreateCorrespondingToBrowser(string browserVersion) =>
            $"{BrowserVersionPrefix}{browserVersion}";

        /// <summary>
        /// Tries to extract browser version from version string.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="browserVersion">The browser version.</param>
        /// <returns><see langword="true"/> if the version string contains a browser version; otherwise, <see langword="false"/>.</returns>
        public static bool TryExtractBrowserVersion(string version, out string browserVersion)
        {
            if (version.StartsWith(BrowserVersionPrefix, StringComparison.Ordinal)
                && version.Length > BrowserVersionPrefix.Length)
            {
                browserVersion = version.Substring(BrowserVersionPrefix.Length);
                return true;
            }
            else
            {
                browserVersion = null;
                return false;
            }
        }
    }
}
