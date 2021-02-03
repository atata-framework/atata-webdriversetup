using System;

namespace Atata.WebDriverSetup
{
    public class DriverSetupConfigurationBuilder : DriverSetupOptionsBuilder<DriverSetupConfigurationBuilder, DriverSetupConfiguration>
    {
        private readonly Func<IHttpRequestExecutor, IDriverSetupStrategy> driverSetupStrategyFactory;

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

        public DriverSetupConfigurationBuilder WithAutoVersion() =>
            WithVersion(DriverVersions.Auto);

        public DriverSetupConfigurationBuilder WithLatestVersion() =>
            WithVersion(DriverVersions.Latest);

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
