namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the result of a driver setup.
/// </summary>
public class DriverSetupResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DriverSetupResult" /> class.
    /// </summary>
    /// <param name="browserName">Name of the browser.</param>
    /// <param name="version">The version.</param>
    /// <param name="directoryPath">The directory path.</param>
    /// <param name="fileName">Name of the file.</param>
    public DriverSetupResult(
        string browserName,
        string version,
        string directoryPath,
        string fileName)
    {
        BrowserName = browserName.CheckNotNullOrWhitespace(nameof(browserName));
        Version = version.CheckNotNullOrWhitespace(nameof(version));
        DirectoryPath = directoryPath.CheckNotNullOrWhitespace(nameof(directoryPath));
        FileName = fileName.CheckNotNullOrWhitespace(nameof(fileName));
    }

    /// <summary>
    /// Gets the name of the browser.
    /// </summary>
    public string BrowserName { get; }

    /// <summary>
    /// Gets the version.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the setup driver directory path.
    /// </summary>
    public string DirectoryPath { get; }

    /// <summary>
    /// Gets the name of the driver file.
    /// </summary>
    public string FileName { get; }
}
