using System;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the driver setup configuration builder.
    /// </summary>
    public class DriverSetupConfigurationBuilder : DriverSetupOptionsBuilder<DriverSetupConfigurationBuilder, DriverSetupConfiguration>
    {
        private readonly Func<IHttpRequestExecutor, IDriverSetupStrategy> driverSetupStrategyFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverSetupConfigurationBuilder"/> class.
        /// </summary>
        /// <param name="browserName">Name of the browser.</param>
        /// <param name="driverSetupStrategyFactory">The driver setup strategy factory.</param>
        /// <param name="context">The driver setup configuration.</param>
        public DriverSetupConfigurationBuilder(
            string browserName,
            Func<IHttpRequestExecutor, IDriverSetupStrategy> driverSetupStrategyFactory,
            DriverSetupConfiguration context)
            : base(context)
        {
            BrowserName = browserName.CheckNotNull(nameof(browserName));
            this.driverSetupStrategyFactory = driverSetupStrategyFactory.CheckNotNull(nameof(driverSetupStrategyFactory));
        }

        /// <summary>
        /// Gets the name of the browser.
        /// </summary>
        public string BrowserName { get; }

        /// <summary>
        /// Sets the automatic driver version detection by installed browser version.
        /// If the version cannot be detected automatically, latest driver version should be used.
        /// </summary>
        /// <returns>The same builder instance.</returns>
        public DriverSetupConfigurationBuilder WithAutoVersion() =>
            WithVersion(DriverVersions.Auto);

        /// <summary>
        /// Sets the latest version of driver.
        /// </summary>
        /// <returns>The same builder instance.</returns>
        public DriverSetupConfigurationBuilder WithLatestVersion() =>
            WithVersion(DriverVersions.Latest);

        /// <summary>
        /// Sets the browser version.
        /// It will find driver version corresponding to the browser version.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <returns>The same builder instance.</returns>
        public DriverSetupConfigurationBuilder ByBrowserVersion(string version) =>
            WithVersion(DriverVersions.CreateCorrespondingToBrowser(version));

        /// <summary>
        /// Sets the version of driver to use.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <returns>The same builder instance.</returns>
        public DriverSetupConfigurationBuilder WithVersion(string version)
        {
            BuildingContext.Version = version.CheckNotNullOrWhitespace(nameof(version));
            return this;
        }

        /// <summary>
        /// Sets up driver.
        /// </summary>
        public void SetUp()
        {
            if (BuildingContext.IsEnabled)
            {
                IHttpRequestExecutor httpRequestExecutor = new ReliableHttpRequestExecutor(
                    new HttpRequestExecutor(BuildingContext.Proxy),
                    BuildingContext.HttpRequestTryCount,
                    BuildingContext.HttpRequestRetryInterval);

                IDriverSetupStrategy setupStrategy = driverSetupStrategyFactory.Invoke(httpRequestExecutor);

                WebDriverSetupExecutor setupExecutor = new WebDriverSetupExecutor(
                    BrowserName,
                    setupStrategy,
                    BuildingContext,
                    httpRequestExecutor);

                if (BuildingContext.Version == DriverVersions.Auto)
                    setupExecutor.SetupCorrespondingOrLatestVersion();
                else if (BuildingContext.Version == DriverVersions.Latest)
                    setupExecutor.SetupLatestVersion();
                else if (DriverVersions.TryExtractBrowserVersion(BuildingContext.Version, out string browserVersion))
                    setupExecutor.SetupByBrowserVersion(browserVersion);
                else
                    setupExecutor.SetUp(BuildingContext.Version);

                DriverSetup.PendingConfigurations.Remove(this);
            }
        }
    }
}
