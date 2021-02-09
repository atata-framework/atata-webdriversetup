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
        private static readonly Dictionary<string, Func<IHttpRequestExecutor, IDriverSetupStrategy>> BrowserStrategyFactoryMap =
            new Dictionary<string, Func<IHttpRequestExecutor, IDriverSetupStrategy>>
            {
                [BrowserNames.Chrome] = hre => new ChromeDriverSetupStrategy(hre),
                [BrowserNames.Firefox] = hre => new FirefoxDriverSetupStrategy(hre),
                [BrowserNames.Edge] = hre => new EdgeDriverSetupStrategy(hre),
                [BrowserNames.Opera] = hre => new OperaDriverSetupStrategy(hre),
                [BrowserNames.InternetExplorer] = hre => new InternetExplorerDriverSetupStrategy()
            };

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
        /// Registers the driver setup strategy factory.
        /// </summary>
        /// <param name="browserName">Name of the browser.</param>
        /// <param name="driverSetupStrategyFactory">The driver setup strategy factory.</param>
        public static void RegisterStrategyFactory(
            string browserName,
            Func<IHttpRequestExecutor, IDriverSetupStrategy> driverSetupStrategyFactory)
        {
            browserName.CheckNotNull(nameof(browserName));
            driverSetupStrategyFactory.CheckNotNull(nameof(driverSetupStrategyFactory));

            BrowserStrategyFactoryMap[browserName] = driverSetupStrategyFactory;
        }

        /// <summary>
        /// Creates the Chrome driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Chrome driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureChrome() =>
            Configure(BrowserNames.Chrome);

        /// <summary>
        /// Creates the Firefox/Gecko driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Firefox driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureFirefox() =>
            Configure(BrowserNames.Firefox);

        /// <summary>
        /// Creates the Edge driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Edge driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureEdge() =>
            Configure(BrowserNames.Edge);

        /// <summary>
        /// Creates the Opera driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Opera driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureOpera() =>
            Configure(BrowserNames.Opera);

        /// <summary>
        /// Creates the Internet Explorer driver setup configuration builder.
        /// </summary>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/> for Internet Explorer driver.</returns>
        public static DriverSetupConfigurationBuilder ConfigureInternetExplorer() =>
            Configure(BrowserNames.InternetExplorer);

        /// <summary>
        /// Creates the driver setup configuration builder for the specified <paramref name="browserName"/>.
        /// Supported browser names are defined in <see cref="BrowserNames"/>.
        /// </summary>
        /// <param name="browserName">The browser name. Can be one of <see cref="BrowserNames"/> values.</param>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/>.</returns>
        public static DriverSetupConfigurationBuilder Configure(string browserName)
        {
            browserName.CheckNotNullOrWhitespace(nameof(browserName));

            return BrowserStrategyFactoryMap.TryGetValue(browserName, out var strategyFactory)
                ? Configure(browserName, strategyFactory)
                : throw new ArgumentException($@"Unsupported ""{browserName}"" browser name.", nameof(browserName));
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
                EnvironmentVariableName = $"{browserName.Replace(" ", null)}Driver"
            };

        /// <summary>
        /// Sets up driver with auto version detection for the browser with the specified name.
        /// Supported browser names are defined in <see cref="BrowserNames" />.
        /// </summary>
        /// <param name="browserName">The browser name. Can be one or many of <see cref="BrowserNames"/> values.</param>
        /// <returns>
        /// The <see cref="DriverSetupResult"/> instance;
        /// or <see langword="null"/>, if the <see cref="DriverSetupOptions.IsEnabled"/> property
        /// of <see cref="GlobalOptions"/> is <see langword="false"/>.
        /// </returns>
        public static DriverSetupResult AutoSetUp(string browserName) =>
            Configure(browserName).WithAutoVersion().SetUp();

        /// <summary>
        /// Sets up drivers with auto version detection for the browsers with the specified names.
        /// Supported browser names are defined in <see cref="BrowserNames"/>.
        /// </summary>
        /// <param name="browserNames">The browser names. Can be one or many of <see cref="BrowserNames"/> values.</param>
        /// <returns>The array of <see cref="DriverSetupResult"/>.</returns>
        public static DriverSetupResult[] AutoSetUp(params string[] browserNames) =>
            AutoSetUp(browserNames?.AsEnumerable());

        /// <summary>
        /// Sets up drivers with auto version detection for the browsers with the specified names.
        /// Supported browser names are defined in <see cref="BrowserNames" />.
        /// </summary>
        /// <param name="browserNames">The browser names. Can be one or many of <see cref="BrowserNames" /> values.</param>
        /// <returns>The array of <see cref="DriverSetupResult"/>.</returns>
        public static DriverSetupResult[] AutoSetUp(IEnumerable<string> browserNames) =>
            browserNames.CheckNotNull(nameof(browserNames))
                .Select(AutoSetUp)
                .Where(res => res != null)
                .ToArray();

        /// <summary>
        /// Sets up drivers with auto version detection for the browsers with the specified names.
        /// Supported browser names are defined in <see cref="BrowserNames" />.
        /// Skips invalid/unsupported browser names.
        /// </summary>
        /// <param name="browserNames">The browser names. Can be one or many of <see cref="BrowserNames" /> values.</param>
        /// <returns>The array of <see cref="DriverSetupResult"/>.</returns>
        public static DriverSetupResult[] AutoSetUpSafely(IEnumerable<string> browserNames) =>
            browserNames != null
                ? AutoSetUp(browserNames.Where(name => BrowserStrategyFactoryMap.ContainsKey(name)))
                : new DriverSetupResult[0];

        /// <summary>
        /// Sets up pending configurations that are stored in <see cref="PendingConfigurations" /> property.
        /// </summary>
        /// <returns>The array of <see cref="DriverSetupResult"/>.</returns>
        public static DriverSetupResult[] SetUpPendingConfigurations() =>
            PendingConfigurations.ToArray()
                .Select(conf => conf.SetUp())
                .Where(res => res != null)
                .ToArray();
    }
}
