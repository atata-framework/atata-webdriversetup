namespace Atata.WebDriverSetup;

/// <inheritdoc/>
public class HttpRequestExecutor : IHttpRequestExecutor
{
    private readonly IWebProxy? _proxy;

    private readonly bool _checkCertificateRevocationList;

    private readonly Action<HttpClientHandler>? _httpClientHandlerConfigurationAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestExecutor"/> class.
    /// </summary>
    /// <param name="proxy">The proxy.</param>
    /// <param name="checkCertificateRevocationList">
    /// A value indicating whether the certificate is automatically picked
    /// from the certificate store or if the caller is allowed to pass in a specific
    /// client certificate.
    /// </param>
    /// <param name="httpClientHandlerConfigurationAction">The configuration action of <see cref="HttpClientHandler"/>.</param>
    public HttpRequestExecutor(
        IWebProxy? proxy = null,
        bool checkCertificateRevocationList = true,
        Action<HttpClientHandler>? httpClientHandlerConfigurationAction = null)
    {
        _proxy = proxy;
        _checkCertificateRevocationList = checkCertificateRevocationList;
        _httpClientHandlerConfigurationAction = httpClientHandlerConfigurationAction;
    }

    /// <inheritdoc/>
    public async Task<string> DownloadStringAsync(string url, CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(true);

        // TODO: After .NET upgrade, use client.GetStringAsync(url, cancellationToken)
        return await client.GetStringAsync(url)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Stream> DownloadStreamAsync(string url, CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(true);

        // TODO: After .NET upgrade, use client.GetStreamAsync(url, cancellationToken)
        return await client.GetStreamAsync(url)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DownloadFileAsync(string url, string filePath, CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(true);
        using HttpResponseMessage response = await client.GetAsync(url, cancellationToken).
            ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        using FileStream fileStream = new(filePath, FileMode.Create);

        // TODO: After .NET upgrade, use response.Content.CopyToAsync(fileStream, cancellationToken)
        await response.Content.CopyToAsync(fileStream)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Uri> GetRedirectUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(false);
        using HttpResponseMessage response = await client.GetAsync(url, cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode != HttpStatusCode.Found)
            throw new HttpRequestException($"""Unexpected HTTP response status for "{url}". Expected 302, but was {(int)response.StatusCode}.""");

        return response.Headers.Location;
    }

    private HttpClient CreateHttpClientWithAutoRedirect(bool allowAutoRedirect)
    {
        HttpClientHandler httpClientHandler = new()
        {
            Proxy = _proxy,
            AllowAutoRedirect = allowAutoRedirect
        };

        if (_checkCertificateRevocationList)
            httpClientHandler.CheckCertificateRevocationList = true;

        _httpClientHandlerConfigurationAction?.Invoke(httpClientHandler);

        return new HttpClient(httpClientHandler, true);
    }
}
