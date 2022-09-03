using System.Collections.Generic;
using System.Linq;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a set of static methods for a detection of browser installations.
    /// Browser detection is supported for Chrome, Firefox and Edge,
    /// so as a browser name the following constants can be used:
    /// <list type="bullet">
    /// <item><see cref="BrowserNames.Chrome"/></item>
    /// <item><see cref="BrowserNames.Firefox"/></item>
    /// <item><see cref="BrowserNames.Edge"/></item>
    /// </list>
    /// </summary>
    public static class BrowserDetector
    {
        /// <inheritdoc cref="GetFirstInstalledBrowserName(IEnumerable{string})"/>
        public static string GetFirstInstalledBrowserName(params string[] browserNames) =>
            GetFirstInstalledBrowserName(browserNames?.AsEnumerable());

        /// <summary>
        /// Gets the name of the first installed browser among the <paramref name="browserNames"/>.
        /// </summary>
        /// <param name="browserNames">The browser names.</param>
        /// <returns>
        /// The browser name; or <see langword="null"/> if none of the browsers is installed.
        /// </returns>
        public static string GetFirstInstalledBrowserName(IEnumerable<string> browserNames) =>
            browserNames.CheckNotNullOrEmpty(nameof(browserNames))
                .FirstOrDefault(x => IsBrowserInstalled(x));

        /// <summary>
        /// Determines whether the browser with the specified name is installed.
        /// </summary>
        /// <param name="browserName">Name of the browser.</param>
        /// <returns>
        /// <see langword="true"/> if the browser is installed; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsBrowserInstalled(string browserName) =>
            GetInstalledBrowserVersion(browserName) != null;

        /// <summary>
        /// Gets the installed browser version by the browser name.
        /// </summary>
        /// <param name="browserName">Name of the browser.</param>
        /// <returns>The version string; or <see langword="null"/> if the browser is not found.</returns>
        public static string GetInstalledBrowserVersion(string browserName)
        {
            browserName.CheckNotNullOrWhitespace(nameof(browserName));

            return DriverSetup.GetInstalledBrowserVersion(browserName);
        }
    }
}
