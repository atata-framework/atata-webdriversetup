namespace Atata.WebDriverSetup;

internal sealed class LoggingHttpRequestExecutor : IHttpRequestExecutor
{
    private readonly IHttpRequestExecutor _httpRequestExecutor;

    public LoggingHttpRequestExecutor(IHttpRequestExecutor httpRequestExecutor) =>
        _httpRequestExecutor = httpRequestExecutor;

    public async Task DownloadFileAsync(string url, string filePath, CancellationToken cancellationToken = default)
    {
        Log.Trace($"Downloading file by URL {url}");

        try
        {
            await _httpRequestExecutor.DownloadFileAsync(url, filePath, cancellationToken)
                .ConfigureAwait(false);

            Log.Trace($"Downloaded file by URL {url} to {filePath}");
        }
        catch (Exception exception)
        {
            Log.Trace($"Failed downloading file by URL {url}: {exception}");
            throw;
        }
    }

    public async Task<string> DownloadStringAsync(string url, CancellationToken cancellationToken = default)
    {
        Log.Trace($"Downloading string by URL {url}");

        try
        {
            var result = await _httpRequestExecutor.DownloadStringAsync(url, cancellationToken)
                .ConfigureAwait(false);

            Log.Trace($"Downloaded string by URL {url}:{(result.Contains('\n') ? Environment.NewLine : " ")}{result}");

            return result;
        }
        catch (Exception exception)
        {
            Log.Trace($"Failed downloading string by URL {url}: {exception}");
            throw;
        }
    }

    public async Task<Stream> DownloadStreamAsync(string url, CancellationToken cancellationToken = default)
    {
        Log.Trace($"Downloading stream by URL {url}");

        try
        {
            var result = await _httpRequestExecutor.DownloadStreamAsync(url, cancellationToken)
                .ConfigureAwait(false);

            Log.Trace($"Downloaded stream by URL {url}");

            return result;
        }
        catch (Exception exception)
        {
            Log.Trace($"Failed downloading stream by URL {url}: {exception}");
            throw;
        }
    }

    public async Task<Uri> GetRedirectUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        Log.Trace($"Getting redirect URL for {url}");

        try
        {
            var result = await _httpRequestExecutor.GetRedirectUrlAsync(url, cancellationToken)
                .ConfigureAwait(false);

            Log.Trace($"Got redirect URL for {url}: {result}");

            return result;
        }
        catch (Exception exception)
        {
            Log.Trace($"Failed getting redirect URL for {url}: {exception}");
            throw;
        }
    }
}
