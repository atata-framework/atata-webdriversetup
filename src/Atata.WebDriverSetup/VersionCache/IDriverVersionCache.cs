namespace Atata.WebDriverSetup;

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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task with the driver version string.</returns>
    ValueTask<string> GetOrAddLatestAsync(
        DateTime minimumAcceptableTimestamp,
        Func<CancellationToken, Task<string>> latestVersionResolveFunction,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a browser/driver versions pair to the cache by using the specified function
    /// if the browser version does not already exist in cache or is timed out,
    /// or returns the existing value if it exists and is not timed out.
    /// </summary>
    /// <param name="browserVersion">The browser version.</param>
    /// <param name="minimumAcceptableTimestamp">The minimum acceptable timestamp.</param>
    /// <param name="versionResolveFunction">The function that resolves the version.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task with the driver version string.</returns>
    ValueTask<string> GetOrAddAsync(
        string browserVersion,
        DateTime minimumAcceptableTimestamp,
        Func<string, CancellationToken, Task<string>> versionResolveFunction,
        CancellationToken cancellationToken = default);
}
