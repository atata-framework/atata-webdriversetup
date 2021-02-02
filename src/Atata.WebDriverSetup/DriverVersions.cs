using System;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a set of predefined version values and a functionality for driver version management.
    /// </summary>
    public static class DriverVersions
    {
        public const string BrowserVersionPrefix = "browser:";

        public const string Auto = "auto";

        public const string Latest = "latest";

        public static string CreateCorrespondingToBrowser(string browserVersion)
        {
            return $"{BrowserVersionPrefix}{browserVersion}";
        }

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
