using System;
using System.IO;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents a functionality for HTTP requests execution.
    /// </summary>
    public interface IHttpRequestExecutor
    {
        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="filePath">The destination file path.</param>
        void DownloadFile(string url, string filePath);

        /// <summary>
        /// Downloads the string.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The downloaded string.</returns>
        string DownloadString(string url);

        /// <summary>
        /// Downloads the stream.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The response stream.</returns>
        Stream DownloadStream(string url);

        /// <summary>
        /// Gets the redirect location URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The redirect location URL.</returns>
        Uri GetRedirectUrl(string url);
    }
}
