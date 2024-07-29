namespace Atata.WebDriverSetup;

internal sealed class LoggingHttpRequestExecutor : IHttpRequestExecutor
{
    private readonly IHttpRequestExecutor _httpRequestExecutor;

    public LoggingHttpRequestExecutor(IHttpRequestExecutor httpRequestExecutor) =>
        _httpRequestExecutor = httpRequestExecutor;

    public void DownloadFile(string url, string filePath)
    {
        Log.Trace($"Downloading file by URL {url}");

        try
        {
            _httpRequestExecutor.DownloadFile(url, filePath);

            Log.Trace($"Downloaded file by URL {url} to {filePath}");
        }
        catch (Exception exception)
        {
            Log.Trace($"Failed downloading file by URL {url}: {exception}");
            throw;
        }
    }

    public string DownloadString(string url)
    {
        Log.Trace($"Downloading string by URL {url}");

        try
        {
            var result = _httpRequestExecutor.DownloadString(url);

            Log.Trace($"Downloaded string by URL {url}:{(result.Contains('\n') ? Environment.NewLine : " ")}{result}");

            return result;
        }
        catch (Exception exception)
        {
            Log.Trace($"Failed downloading string by URL {url}: {exception}");
            throw;
        }
    }

    public Stream DownloadStream(string url)
    {
        Log.Trace($"Downloading stream by URL {url}");

        try
        {
            var result = _httpRequestExecutor.DownloadStream(url);

            Log.Trace($"Downloaded stream by URL {url}");

            return result;
        }
        catch (Exception exception)
        {
            Log.Trace($"Failed downloading stream by URL {url}: {exception}");
            throw;
        }
    }

    public Uri GetRedirectUrl(string url)
    {
        Log.Trace($"Getting redirect URL for {url}");

        try
        {
            var result = _httpRequestExecutor.GetRedirectUrl(url);

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
