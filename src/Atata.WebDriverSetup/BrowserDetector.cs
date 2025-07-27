namespace Atata.WebDriverSetup;

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
    public static string? GetFirstInstalledBrowserName(params string[] browserNames) =>
        GetFirstInstalledBrowserName(browserNames?.AsEnumerable()!);

    /// <summary>
    /// Gets the name of the first installed browser among the <paramref name="browserNames"/>.
    /// </summary>
    /// <param name="browserNames">The browser names.</param>
    /// <returns>
    /// The browser name; or <see langword="null"/> if none of the browsers is installed.
    /// </returns>
    public static string? GetFirstInstalledBrowserName(IEnumerable<string> browserNames)
    {
        Guard.ThrowIfNullOrEmpty(browserNames);

        return browserNames.FirstOrDefault(IsBrowserInstalled);
    }

    /// <summary>
    /// Determines whether the browser with the specified name is installed.
    /// </summary>
    /// <param name="browserName">Name of the browser.</param>
    /// <returns>
    /// <see langword="true"/> if the browser is installed; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsBrowserInstalled(string browserName) =>
        IsBrowserInstalledAsync(browserName).GetAwaiter().GetResult();

    /// <summary>
    /// Determines whether the browser with the specified name is installed.
    /// </summary>
    /// <param name="browserName">Name of the browser.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task with a value <see langword="true"/> if the browser is installed; otherwise, <see langword="false"/>.
    /// </returns>
    public static async Task<bool> IsBrowserInstalledAsync(string browserName, CancellationToken cancellationToken = default) =>
        (await GetInstalledBrowserVersionAsync(browserName, cancellationToken).ConfigureAwait(false)) is not null;

    /// <summary>
    /// Gets the installed browser version by the browser name.
    /// </summary>
    /// <param name="browserName">Name of the browser.</param>
    /// <returns>The version string; or <see langword="null"/> if the browser is not found.</returns>
    public static string? GetInstalledBrowserVersion(string browserName) =>
        GetInstalledBrowserVersionAsync(browserName).GetAwaiter().GetResult();

    /// <summary>
    /// Gets the installed browser version by the browser name.
    /// </summary>
    /// <param name="browserName">Name of the browser.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with the version string; or <see langword="null"/> if the browser is not found.</returns>
    public static async Task<string?> GetInstalledBrowserVersionAsync(string browserName, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrWhitespace(browserName);

        return await DriverSetup.GetInstalledBrowserVersionAsync(browserName, cancellationToken)
            .ConfigureAwait(false);
    }
}
