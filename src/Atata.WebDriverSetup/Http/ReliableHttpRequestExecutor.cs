using System;
using System.IO;
using System.Threading.Tasks;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents a reliable HTTP request executor.
    /// </summary>
    public class ReliableHttpRequestExecutor : IHttpRequestExecutor
    {
        private readonly IHttpRequestExecutor _httpRequestExecutor;

        private readonly int _tryCount;

        private readonly TimeSpan _retryInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableHttpRequestExecutor"/> class.
        /// </summary>
        /// <param name="httpRequestExecutor">The HTTP request executor.</param>
        /// <param name="tryCount">The try count.</param>
        /// <param name="retryInterval">The retry interval.</param>
        public ReliableHttpRequestExecutor(
            IHttpRequestExecutor httpRequestExecutor,
            int tryCount,
            TimeSpan retryInterval)
        {
            _httpRequestExecutor = httpRequestExecutor;
            _tryCount = tryCount;
            _retryInterval = retryInterval;
        }

        /// <inheritdoc/>
        public void DownloadFile(string url, string filePath) =>
            ExecuteWithRetries(() =>
            {
                _httpRequestExecutor.DownloadFile(url, filePath);
                return true;
            });

        /// <inheritdoc/>
        public string DownloadString(string url) =>
            ExecuteWithRetries(() =>
                _httpRequestExecutor.DownloadString(url));

        /// <inheritdoc/>
        public Stream DownloadStream(string url) =>
            ExecuteWithRetries(() =>
                _httpRequestExecutor.DownloadStream(url));

        /// <inheritdoc/>
        public Uri GetRedirectUrl(string url) =>
            ExecuteWithRetries(() =>
                _httpRequestExecutor.GetRedirectUrl(url));

        private TResult ExecuteWithRetries<TResult>(Func<TResult> operation)
        {
            int attempt = 1;

            while (true)
            {
                try
                {
                    return operation.Invoke();
                }
                catch (Exception)
                {
                    if (attempt >= _tryCount)
                        throw;

                    attempt++;
                    Task.Delay(_retryInterval).Wait();
                }
            }
        }
    }
}
