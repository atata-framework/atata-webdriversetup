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
    public string DownloadString(string url)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(true);

        return client.GetStringAsync(url).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public Stream DownloadStream(string url)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(true);

        return client.GetStreamAsync(url).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void DownloadFile(string url, string filePath)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(true);
        using HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();

        response.EnsureSuccessStatusCode();

        using FileStream fileStream = new(filePath, FileMode.Create);
        response.Content.CopyToAsync(fileStream).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public Uri GetRedirectUrl(string url)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(false);
        using HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();

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
