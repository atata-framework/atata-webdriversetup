using System;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a set of methods for driver version resolution using cache.
    /// </summary>
    public interface IDriverVersionCache
    {
        /// <summary>
        /// Adds a latest driver version to the cache by using the specified function
        /// if the latest version does not already exist in cache or is timed out,
        /// or returns the existing value if it exists and is not timed out.
        /// </summary>
        /// <param name="minimumAcceptableTimestamp">The minimum acceptable timestamp.</param>
        /// <param name="latestVersionResolveFunction">The function that resolves the latest version.</param>
        /// <returns>The driver version string.</returns>
        string GetOrAddLatest(DateTime minimumAcceptableTimestamp, Func<string> latestVersionResolveFunction);

        /// <summary>
        /// Adds a browser/driver versions pair to the cache by using the specified function
        /// if the browser version does not already exist in cache or is timed out,
        /// or returns the existing value if it exists and is not timed out.
        /// </summary>
        /// <param name="browserVersion">The browser version.</param>
        /// <param name="minimumAcceptableTimestamp">The minimum acceptable timestamp.</param>
        /// <param name="versionResolveFunction">The function that resolves the version.</param>
        /// <returns>The driver version string.</returns>
        string GetOrAdd(string browserVersion, DateTime minimumAcceptableTimestamp, Func<string, string> versionResolveFunction);
    }
}
