using System;
using System.Linq;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a set of utility methods for version strings.
    /// </summary>
    public static class VersionUtils
    {
        /// <summary>
        /// Trims the revision number of the version.
        /// </summary>
        /// <example>
        /// <c>"1.2.3.4"</c> -> <c>"1.2.3"</c>.
        /// </example>
        /// <param name="version">The version.</param>
        /// <returns>The trimmed version string.</returns>
        public static string TrimRevision(string version) =>
            CutVersion(version, 3);

        /// <summary>
        /// Trims the patch and revision numbers of the version.
        /// </summary>
        /// <example>
        /// <c>"1.2.3.4"</c> -> <c>"1.2"</c>.
        /// </example>
        /// <param name="version">The version.</param>
        /// <returns>The trimmed version string.</returns>
        public static string TrimPatch(string version) =>
            CutVersion(version, 2);

        /// <summary>
        /// Trims the minor, patch and revision numbers of the version.
        /// </summary>
        /// <example>
        /// <c>"1.2.3.4"</c> -> <c>"1"</c>.
        /// </example>
        /// <param name="version">The version.</param>
        /// <returns>The trimmed version string.</returns>
        public static string TrimMinor(string version) =>
            CutVersion(version, 1);

        private static string CutVersion(string version, int versionNumbersToLeave)
        {
            version.CheckNotNullOrWhitespace(nameof(version));

            string[] parts = version.Split('.');

            if (parts.Length < versionNumbersToLeave)
                throw new ArgumentException($@"Inappropriate ""{version}"" {nameof(version)} format. Version should consist at least of {versionNumbersToLeave} numbers.", nameof(version));

            return string.Join(".", parts.Take(versionNumbersToLeave));
        }

        /// <summary>
        /// Gets the version numbers count.
        /// </summary>
        /// <example>
        /// <c>"1.2.3.4"</c> -> 4.
        /// </example>
        /// <param name="version">The version.</param>
        /// <returns>The count of numbers.</returns>
        public static int GetNumbersCount(string version)
        {
            version.CheckNotNullOrWhitespace(nameof(version));

            return version.Count(x => x == '.') + 1;
        }
    }
}
