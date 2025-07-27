namespace Atata.WebDriverSetup;

/// <summary>
/// Represents a reliable HTTP request executor.
/// </summary>
public class ReliableHttpRequestExecutor : IHttpRequestExecutor
{
    private readonly IHttpRequestExecutor _httpRequestExecutor;

    private readonly int _tryCount;

    private readonly TimeSpan _retryInterval;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReliableHttpRequestExecutor"/> class.
    /// </summary>
    /// <param name="httpRequestExecutor">The HTTP request executor.</param>
    /// <param name="tryCount">The try count.</param>
    /// <param name="retryInterval">The retry interval.</param>
    public ReliableHttpRequestExecutor(
        IHttpRequestExecutor httpRequestExecutor,
        int tryCount,
        TimeSpan retryInterval)
    {
        _httpRequestExecutor = httpRequestExecutor;
        _tryCount = tryCount;
        _retryInterval = retryInterval;
    }

    /// <inheritdoc/>
    public async Task DownloadFileAsync(string url, string filePath, CancellationToken cancellationToken = default) =>
        await ExecuteWithRetriesAsync(
            async ct =>
            {
                await _httpRequestExecutor.DownloadFileAsync(url, filePath, ct)
                    .ConfigureAwait(false);
                return true;
            },
            cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<string> DownloadStringAsync(string url, CancellationToken cancellationToken = default) =>
        await ExecuteWithRetriesAsync(
            ct => _httpRequestExecutor.DownloadStringAsync(url, ct),
            cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Stream> DownloadStreamAsync(string url, CancellationToken cancellationToken = default) =>
        await ExecuteWithRetriesAsync(
            ct => _httpRequestExecutor.DownloadStreamAsync(url, ct),
            cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Uri> GetRedirectUrlAsync(string url, CancellationToken cancellationToken = default) =>
        await ExecuteWithRetriesAsync(
            ct => _httpRequestExecutor.GetRedirectUrlAsync(url, ct),
            cancellationToken)
            .ConfigureAwait(false);

    private async Task<TResult> ExecuteWithRetriesAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken)
    {
        int attempt = 1;

        while (true)
        {
            try
            {
                return await operation.Invoke(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                if (attempt >= _tryCount)
                    throw;

                attempt++;

                await Task.Delay(_retryInterval, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
