namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the specific driver setup configuration.
    /// </summary>
    public class DriverSetupConfiguration : DriverSetupOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverSetupConfiguration"/> class.
        /// </summary>
        public DriverSetupConfiguration()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverSetupConfiguration"/> class using <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options.</param>
        public DriverSetupConfiguration(DriverSetupOptions options)
        {
            if (options != null)
            {
                StorageDirectoryPath = options.StorageDirectoryPath;
                Proxy = options.Proxy;
                UseVersionCache = options.UseVersionCache;
                LatestVersionCheckInterval = options.LatestVersionCheckInterval;
                SpecificVersionCheckInterval = options.SpecificVersionCheckInterval;
                HttpRequestTryCount = options.HttpRequestTryCount;
                HttpRequestRetryInterval = options.HttpRequestRetryInterval;
                IsEnabled = options.IsEnabled;
                AddToEnvironmentPathVariable = options.AddToEnvironmentPathVariable;
            }
        }

        /// <summary>
        /// Gets or sets the version.
        /// The default value is <see cref="DriverVersions.Auto"/>.
        /// </summary>
        public string Version { get; set; } = DriverVersions.Auto;

        /// <summary>
        /// <para>
        /// Gets or sets the name of the environment variable
        /// that will be set with a value equal to the driver directory path.
        /// </para>
        /// <para>
        /// The default value is specific to the driver being configured.
        /// It has <c>"Atata.{BrowserName}Driver"</c> format.
        /// For example: <c>"Atata.ChromeDriver"</c> or <c>"Atata.InternetExplorerDriver"</c>.
        /// </para>
        /// <para>
        /// The <see langword="null"/> value means that none variable should be set.
        /// </para>
        /// </summary>
        public string EnvironmentVariableName { get; set; }
    }
}
