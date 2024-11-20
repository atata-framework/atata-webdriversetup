namespace Atata.WebDriverSetup.IntegrationTests;

internal class FakeHttpRequestExecutorProxy : IHttpRequestExecutor
{
    private readonly IHttpRequestExecutor _realExecutor;

    private readonly List<(string Url, string FilePath)> _downloadFileCalls = [];

    public FakeHttpRequestExecutorProxy()
        : this(new HttpRequestExecutor())
    {
    }

    public FakeHttpRequestExecutorProxy(IHttpRequestExecutor realExecutor) =>
        _realExecutor = realExecutor;

    public int DownloadFileFailuresCount { get; set; }

    public IReadOnlyList<(string Url, string FilePath)> DownloadFileCalls =>
        _downloadFileCalls;

    public Queue<Func<Func<string>, string>> DownloadStringInterceptions { get; } = [];

    public void DownloadFile(string url, string filePath)
    {
        _downloadFileCalls.Add((url, filePath));

        if (DownloadFileFailuresCount > 0)
        {
            DownloadFileFailuresCount--;

            throw new HttpRequestException("Fake 404");
        }
        else
        {
            _realExecutor.DownloadFile(url, filePath);
        }
    }

    public Stream DownloadStream(string url) =>
        _realExecutor.DownloadStream(url);

    public string DownloadString(string url) =>
        DownloadStringInterceptions.TryDequeue(out var interceptor)
            ? interceptor.Invoke(() => _realExecutor.DownloadString(url))
            : _realExecutor.DownloadString(url);

    public Uri GetRedirectUrl(string url) =>
        _realExecutor.GetRedirectUrl(url);
}
