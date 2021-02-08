using System;
using System.IO;
using System.Net;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the common driver setup options.
    /// </summary>
    public class DriverSetupOptions
    {
        /// <summary>
        /// Gets or sets the storage directory path.
        /// The default value is <c>"{basedir}/drivers")</c>.
        /// </summary>
        public string StorageDirectoryPath { get; set; } =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "drivers");

        /// <summary>
        /// Gets or sets the proxy.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use version cache.
        /// The default value is <see langword="true"/>.
        /// </summary>
        public bool UseVersionCache { get; set; } = true;

        /// <summary>
        /// Gets or sets the latest version check interval.
        /// The default value is <c>2</c> hours.
        /// </summary>
        public TimeSpan LatestVersionCheckInterval { get; set; } = TimeSpan.FromHours(2);

        /// <summary>
        /// Gets or sets the specific version check interval.
        /// The default value is <c>2</c> hours.
        /// </summary>
        public TimeSpan SpecificVersionCheckInterval { get; set; } = TimeSpan.FromHours(2);

        /// <summary>
        /// Gets or sets the HTTP request try count.
        /// The default value is <c>3</c>.
        /// </summary>
        public int HttpRequestTryCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets the HTTP request retry interval.
        /// The default value is <c>3</c> seconds.
        /// </summary>
        public TimeSpan HttpRequestRetryInterval { get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// The default value is <see langword="true"/>.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to add the driver directory path
        /// to environment "Path" variable.
        /// The default value is <see langword="true"/>.
        /// </summary>
        public bool AddToEnvironmentPathVariable { get; set; } = true;
    }
}
