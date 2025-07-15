namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a method that resolves the installed browser version.
/// </summary>
public interface IGetsInstalledBrowserVersion
{
    /// <summary>
    /// Gets the installed browser version.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The version number or <see langword="null"/>.</returns>
    Task<string?> GetInstalledBrowserVersionAsync(CancellationToken cancellationToken = default);
}
