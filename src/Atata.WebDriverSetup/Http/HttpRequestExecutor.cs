﻿using System;
using System.Net;
using System.Net.Http;

namespace Atata.WebDriverSetup
{
    /// <inheritdoc/>
    public class HttpRequestExecutor : IHttpRequestExecutor
    {
        private readonly IWebProxy proxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestExecutor"/> class.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        public HttpRequestExecutor(IWebProxy proxy = null)
        {
            this.proxy = proxy;
        }

        /// <inheritdoc/>
        public string DownloadString(string url)
        {
            using (var webClient = CreateWebClient())
            {
                return webClient.DownloadString(url);
            }
        }

        /// <inheritdoc/>
        public void DownloadFile(string url, string filePath)
        {
            using (var webClient = CreateWebClient())
            {
                webClient.DownloadFile(url, filePath);
            }
        }

        /// <inheritdoc/>
        public Uri GetRedirectUrl(string url)
        {
            using (HttpClient client = CreateHttpClient())
            using (HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult())
            {
                if (response.StatusCode != HttpStatusCode.Found)
                    throw new HttpRequestException($@"Unexpected HTTP response status for ""{url}"". Expected 302, but was {(int)response.StatusCode}.");

                return response.Headers.Location;
            }
        }

        private WebClient CreateWebClient()
        {
            WebClient webClient = new WebClient();

            if (proxy != null)
                webClient.Proxy = proxy;

            return webClient;
        }

        private HttpClient CreateHttpClient()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
                AllowAutoRedirect = false
            };
            HttpClient httpClient = new HttpClient(httpClientHandler, true);

            return httpClient;
        }
    }
}
