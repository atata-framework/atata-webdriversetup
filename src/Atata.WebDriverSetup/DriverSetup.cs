using System;
using System.Collections.Generic;
using System.Linq;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a set of static methods and properties for driver configuration and setup.
    /// </summary>
    public static class DriverSetup
    {
        /// <summary>
        /// Gets the global setup options.
        /// </summary>
        public static DriverSetupOptions GlobalOptions { get; } =
            new DriverSetupOptions();

        /// <summary>
        /// Gets the global setup configuration builder.
        /// Configures <see cref="GlobalOptions"/>.
        /// </summary>
        public static DriverSetupOptionsBuilder GlobalConfiguration { get; } =
            new DriverSetupOptionsBuilder(GlobalOptions);

        /// <summary>
        /// Gets the pending driver setup configurations,
        /// the configurations that were created but were not set up.
        /// </summary>
        public static List<DriverSetupConfigurationBuilder> PendingConfigurations { get; } =
            new List<DriverSetupConfigurationBuilder>();

        /// <summary>
        /// Creates the Chrome driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Chrome driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureChrome() =>
            Configure(BrowserNames.Chrome, hre => new ChromeDriverSetupStrategy(hre));

        /// <summary>
        /// Creates the Firefox driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Firefox driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureFirefox() =>
            Configure(BrowserNames.Firefox, hre => new FirefoxDriverSetupStrategy(hre));

        /// <summary>
        /// Creates the Edge driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Edge driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureEdge() =>
            Configure(BrowserNames.Edge, hre => new EdgeDriverSetupStrategy(hre));

        /// <summary>
        /// Creates the Opera driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Opera driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureOpera() =>
            Configure(BrowserNames.Opera, hre => new OperaDriverSetupStrategy(hre));

        /// <summary>
        /// Creates the Internet Explorer driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Internet Explorer driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureInternetExplorer() =>
            Configure(BrowserNames.InternetExplorer, hre => new InternetExplorerDriverSetupStrategy());

        /// <summary>
        /// Creates the driver setup configuration builder for the specified <paramref name="browserName"/>.
        /// Supported browser names are defined in <see cref="BrowserNames"/>.
        /// </summary>
        /// <param name="browserName">The browser name. Can be one of <see cref="BrowserNames"/> values.</param>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/>.</returns>
        public static DriverSetupConfigurationBuilder Configure(string browserName)
        {
            switch (browserName)
            {
                case BrowserNames.Chrome:
                    return ConfigureChrome();
                case BrowserNames.Firefox:
                    return ConfigureFirefox();
                case BrowserNames.Edge:
                    return ConfigureEdge();
                case BrowserNames.Opera:
                    return ConfigureOpera();
                case BrowserNames.InternetExplorer:
                    return ConfigureInternetExplorer();
                default:
                    throw new ArgumentException($@"Unsupported ""{browserName}"" browser name.", nameof(browserName));
            }
        }

        /// <summary>
        /// Creates the driver setup configuration builder using <paramref name="driverSetupStrategyFactory"/>
        /// that instantiates specific <see cref="IDriverSetupStrategy"/>.
        /// </summary>
        /// <param name="browserName">The name of the browser.</param>
        /// <param name="driverSetupStrategyFactory">The driver setup strategy factory.</param>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/>.</returns>
        public static DriverSetupConfigurationBuilder Configure(
            string browserName,
            Func<IHttpRequestExecutor, IDriverSetupStrategy> driverSetupStrategyFactory)
        {
            var builder = new DriverSetupConfigurationBuilder(
                browserName,
                driverSetupStrategyFactory,
                CreateConfiguration(browserName));

            PendingConfigurations.Add(builder);

            return builder;
        }

        private static DriverSetupConfiguration CreateConfiguration(string browserName) =>
            new DriverSetupConfiguration(GlobalConfiguration.BuildingContext)
            {
                EnvironmentVariableName = $"Atata.{browserName.Replace(" ", null)}Driver"
            };

        /// <summary>
        /// Sets up driver with auto version detecting for the browsers with the specified names.
        /// Supported browser names are defined in <see cref="BrowserNames"/>.
        /// </summary>
        /// <param name="browserNames">The browser names. Can be one or many of <see cref="BrowserNames"/> values.</param>
        public static void AutoSetUp(params string[] browserNames)
        {
            AutoSetUp(browserNames.AsEnumerable());
        }

        /// <summary>
        /// Sets up driver with auto version detecting for the browsers with the specified names.
        /// Supported browser names are defined in <see cref="BrowserNames"/>.
        /// </summary>
        /// <param name="browserNames">The browser names. Can be one or many of <see cref="BrowserNames"/> values.</param>
        public static void AutoSetUp(IEnumerable<string> browserNames)
        {
            browserNames.CheckNotNull(nameof(browserNames));

            foreach (string browserName in browserNames)
                Configure(browserName).WithAutoVersion().SetUp();
        }

        /// <summary>
        /// Sets up pending configurations that are stored in <see cref="PendingConfigurations"/> property.
        /// </summary>
        public static void SetUpPendingConfigurations()
        {
            foreach (var configuration in PendingConfigurations.ToArray())
                configuration.SetUp();
        }
    }
}
