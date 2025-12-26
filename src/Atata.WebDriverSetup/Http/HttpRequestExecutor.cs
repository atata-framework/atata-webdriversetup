namespace Atata.WebDriverSetup;

/// <inheritdoc/>
public class HttpRequestExecutor : IHttpRequestExecutor
{
    private readonly Action<HttpClientHandler>? _httpClientHandlerConfigurationAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestExecutor"/> class.
    /// </summary>
    /// <param name="httpClientHandlerConfigurationAction">The configuration action of <see cref="HttpClientHandler"/>.</param>
    public HttpRequestExecutor(Action<HttpClientHandler>? httpClientHandlerConfigurationAction = null) =>
        _httpClientHandlerConfigurationAction = httpClientHandlerConfigurationAction;

    /// <inheritdoc/>
    public async Task<string> DownloadStringAsync(string url, CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(true);

#if NET8_0_OR_GREATER
        return await client.GetStringAsync(url, cancellationToken)
#else
        return await client.GetStringAsync(url)
#endif
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Stream> DownloadStreamAsync(string url, CancellationToken cancellationToken = default)
    {
        using HttpClient client = CreateHttpClientWithAutoRedirect(true);

#if NET8_0_OR_GREATER
        return await client.GetStreamAsync(url, cancellationToken)
#else
        return await client.GetStreamAsync(url)
#endif
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

#if NET8_0_OR_GREATER
        await response.Content.CopyToAsync(fileStream, cancellationToken)
#else
        await response.Content.CopyToAsync(fileStream)
#endif
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

        return response.Headers.Location
            ?? throw new HttpRequestException("Unexpected HTTP response null location header.");
    }

    private HttpClient CreateHttpClientWithAutoRedirect(bool allowAutoRedirect)
    {
        HttpClientHandler httpClientHandler = new()
        {
            AllowAutoRedirect = allowAutoRedirect
        };

        _httpClientHandlerConfigurationAction?.Invoke(httpClientHandler);

        return new(httpClientHandler, true);
    }
}
