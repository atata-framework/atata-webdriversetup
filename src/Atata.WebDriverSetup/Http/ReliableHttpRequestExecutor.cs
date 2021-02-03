using System;
using System.Threading.Tasks;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents a reliable HTTP request executor.
    /// </summary>
    public class ReliableHttpRequestExecutor : IHttpRequestExecutor
    {
        private readonly IHttpRequestExecutor httpRequestExecutor;

        private readonly int tryCount;

        private readonly TimeSpan retryInterval;

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
            this.httpRequestExecutor = httpRequestExecutor;
            this.tryCount = tryCount;
            this.retryInterval = retryInterval;
        }

        /// <inheritdoc/>
        public void DownloadFile(string url, string filePath)
        {
            ExecuteWithRetries(() =>
            {
                httpRequestExecutor.DownloadFile(url, filePath);
                return true;
            });
        }

        /// <inheritdoc/>
        public string DownloadString(string url)
        {
            return ExecuteWithRetries(() =>
                httpRequestExecutor.DownloadString(url));
        }

        /// <inheritdoc/>
        public Uri GetRedirectUrl(string url)
        {
            return ExecuteWithRetries(() =>
                httpRequestExecutor.GetRedirectUrl(url));
        }

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
                    if (attempt >= tryCount)
                        throw;

                    attempt++;
                    Task.Delay(retryInterval).Wait();
                }
            }
        }
    }
}
