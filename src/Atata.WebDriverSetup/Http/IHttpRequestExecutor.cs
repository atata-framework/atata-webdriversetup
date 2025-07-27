namespace Atata.WebDriverSetup;

/// <summary>
/// Represents a functionality for HTTP requests execution.
/// </summary>
public interface IHttpRequestExecutor
{
    /// <summary>
    /// Downloads the file.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task.</returns>
    Task DownloadFileAsync(string url, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the string.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with the downloaded string.</returns>
    Task<string> DownloadStringAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the stream.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with the response stream.</returns>
    Task<Stream> DownloadStreamAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the redirect location URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with the redirect location URL.</returns>
    Task<Uri> GetRedirectUrlAsync(string url, CancellationToken cancellationToken = default);
}
