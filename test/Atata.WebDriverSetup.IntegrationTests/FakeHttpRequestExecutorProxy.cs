namespace Atata.WebDriverSetup.IntegrationTests;

internal class FakeHttpRequestExecutorProxy : IHttpRequestExecutor
{
    private readonly IHttpRequestExecutor _realExecutor;

    public FakeHttpRequestExecutorProxy(IHttpRequestExecutor realExecutor) =>
        _realExecutor = realExecutor;

    public int DownloadFileFailuresCount { get; set; }

    public List<(string Url, string FilePath)> DownloadFileCalls { get; } = [];

    public void DownloadFile(string url, string filePath)
    {
        DownloadFileCalls.Add((url, filePath));

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
        _realExecutor.DownloadString(url);

    public Uri GetRedirectUrl(string url) =>
        _realExecutor.GetRedirectUrl(url);
}
