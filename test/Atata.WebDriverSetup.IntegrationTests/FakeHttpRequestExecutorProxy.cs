namespace Atata.WebDriverSetup.IntegrationTests;

internal sealed class FakeHttpRequestExecutorProxy : IHttpRequestExecutor
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

    public Queue<Func<Func<Task<string>>, Task<string>>> DownloadStringInterceptions { get; } = [];

    public async Task DownloadFileAsync(string url, string filePath, CancellationToken cancellationToken = default)
    {
        _downloadFileCalls.Add((url, filePath));

        if (DownloadFileFailuresCount > 0)
        {
            DownloadFileFailuresCount--;

            throw new HttpRequestException("Fake 404");
        }
        else
        {
            await _realExecutor.DownloadFileAsync(url, filePath, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public async Task<Stream> DownloadStreamAsync(string url, CancellationToken cancellationToken = default) =>
        await _realExecutor.DownloadStreamAsync(url, cancellationToken)
            .ConfigureAwait(false);

    public async Task<string> DownloadStringAsync(string url, CancellationToken cancellationToken = default)
    {
        var interceptor = DownloadStringInterceptions.Count > 0
            ? DownloadStringInterceptions.Dequeue()
            : null;

        return interceptor is not null
            ? await interceptor.Invoke(async () => await _realExecutor.DownloadStringAsync(url, cancellationToken).ConfigureAwait(false))
                .ConfigureAwait(false)
            : await _realExecutor.DownloadStringAsync(url, cancellationToken)
                .ConfigureAwait(false);
    }

    public async Task<Uri> GetRedirectUrlAsync(string url, CancellationToken cancellationToken = default) =>
        await _realExecutor.GetRedirectUrlAsync(url, cancellationToken)
            .ConfigureAwait(false);
}
