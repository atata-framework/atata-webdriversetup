using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Atata.WebDriverSetup
{
    /// <inheritdoc/>
    public class HttpRequestExecutor : IHttpRequestExecutor
    {
        private readonly IWebProxy _proxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestExecutor"/> class.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        public HttpRequestExecutor(IWebProxy proxy = null) =>
            _proxy = proxy;

        /// <inheritdoc/>
        public string DownloadString(string url)
        {
            using (HttpClient client = CreateHttpClientWithAutoRedirect(true))
            {
                return client.GetStringAsync(url).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc/>
        public void DownloadFile(string url, string filePath)
        {
            using (HttpClient client = CreateHttpClientWithAutoRedirect(true))
            using (HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult())
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                response.Content.CopyToAsync(fileStream).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc/>
        public Uri GetRedirectUrl(string url)
        {
            using (HttpClient client = CreateHttpClientWithAutoRedirect(false))
            using (HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult())
            {
                if (response.StatusCode != HttpStatusCode.Found)
                    throw new HttpRequestException($@"Unexpected HTTP response status for ""{url}"". Expected 302, but was {(int)response.StatusCode}.");

                return response.Headers.Location;
            }
        }

        private HttpClient CreateHttpClientWithAutoRedirect(bool allowAutoRedirect)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                Proxy = _proxy,
                AllowAutoRedirect = allowAutoRedirect,
                CheckCertificateRevocationList = true
            };
            return new HttpClient(httpClientHandler, true);
        }
    }
}
