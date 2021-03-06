﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a set of static methods and properties for driver configuration and setup.
    /// </summary>
    public static class DriverSetup
    {
        private static readonly Dictionary<string, DriverSetupData> BrowserDriverSetupDataMap =
            new Dictionary<string, DriverSetupData>();

        static DriverSetup()
        {
            RegisterStrategyFactory(BrowserNames.Chrome, hre => new ChromeDriverSetupStrategy(hre));
            RegisterStrategyFactory(BrowserNames.Firefox, hre => new FirefoxDriverSetupStrategy(hre));
            RegisterStrategyFactory(BrowserNames.Edge, hre => new EdgeDriverSetupStrategy(hre));
            RegisterStrategyFactory(BrowserNames.Opera, hre => new OperaDriverSetupStrategy(hre));
            RegisterStrategyFactory(BrowserNames.InternetExplorer, hre => new InternetExplorerDriverSetupStrategy());
        }

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

            DriverSetupOptionsBuilder optionsBuilder = BrowserDriverSetupDataMap.TryGetValue(browserName, out DriverSetupData currentData)
                ? currentData.DefaultOptionsBuilder
                : new DriverSetupOptionsBuilder(new DriverSetupOptions(GlobalOptions));

            BrowserDriverSetupDataMap[browserName] = new DriverSetupData(driverSetupStrategyFactory, optionsBuilder);
        }

        /// <summary>
        /// Gets the default driver setup configuration builder.
        /// </summary>
        /// <param name="browserName">Name of the browser.</param>
        /// <example>
        /// Can be used to set, for example, default x32 architecture for Internet Explorer driver.
        /// <code>
        /// DriverSetup.GetDefaultConfiguration(BrowserNames.InternetExplorer)
        ///    .WithX32Architecture();
        /// </code>
        /// </example>
        /// <returns>The <see cref="DriverSetupOptionsBuilder"/> instance.</returns>
        public static DriverSetupOptionsBuilder GetDefaultConfiguration(string browserName) =>
            GetDriverSetupData(browserName).DefaultOptionsBuilder;

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
        /// Supported browser names are defined in <see cref="BrowserNames"/> static class.
        /// </summary>
        /// <param name="browserName">The browser name. Can be one of <see cref="BrowserNames"/> values.</param>
        /// <returns>The <see cref="DriverSetupConfigurationBuilder"/>.</returns>
        public static DriverSetupConfigurationBuilder Configure(string browserName)
        {
            DriverSetupData driverSetupData = GetDriverSetupData(browserName);

            return Configure(browserName, driverSetupData.StrategyFactory, driverSetupData.DefaultOptionsBuilder.BuildingContext);
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
            DriverSetupOptions driverSetupOptions = BrowserDriverSetupDataMap.TryGetValue(browserName, out DriverSetupData driverSetupData)
                ? driverSetupData.DefaultOptionsBuilder.BuildingContext
                : GlobalOptions;

            return Configure(browserName, driverSetupStrategyFactory, driverSetupOptions);
        }

        private static DriverSetupConfigurationBuilder Configure(
            string browserName,
            Func<IHttpRequestExecutor, IDriverSetupStrategy> driverSetupStrategyFactory,
            DriverSetupOptions driverSetupOptions)
        {
            var builder = new DriverSetupConfigurationBuilder(
                browserName,
                driverSetupStrategyFactory,
                CreateConfiguration(browserName, driverSetupOptions));

            PendingConfigurations.Add(builder);

            return builder;
        }

        private static DriverSetupData GetDriverSetupData(string browserName)
        {
            browserName.CheckNotNullOrWhitespace(nameof(browserName));

            return BrowserDriverSetupDataMap.TryGetValue(browserName, out DriverSetupData setupData)
                ? setupData
                : throw new ArgumentException($@"Unsupported ""{browserName}"" browser name.", nameof(browserName));
        }

        private static DriverSetupConfiguration CreateConfiguration(
            string browserName,
            DriverSetupOptions driverSetupOptions) =>
            new DriverSetupConfiguration(driverSetupOptions)
            {
                EnvironmentVariableName = $"{browserName.Replace(" ", null)}Driver"
            };

        /// <summary>
        /// Sets up driver with auto version detection for the browser with the specified name.
        /// Supported browser names are defined in <see cref="BrowserNames"/> static class.
        /// </summary>
        /// <param name="browserName">The browser name. Can be one or many of <see cref="BrowserNames"/> values.</param>
        /// <returns>
        /// The <see cref="DriverSetupResult"/> instance;
        /// or <see langword="null"/>, if the <see cref="DriverSetupOptions.IsEnabled"/> property
        /// of <see cref="GlobalOptions"/> is <see langword="false"/>.
        /// </returns>
        public static DriverSetupResult AutoSetUp(string browserName) =>
            Configure(browserName).WithAutoVersion().SetUp();

        /// <inheritdoc cref="AutoSetUp(IEnumerable{string})"/>
        public static DriverSetupResult[] AutoSetUp(params string[] browserNames) =>
            AutoSetUp(browserNames?.AsEnumerable());

        /// <summary>
        /// Sets up drivers with auto version detection for the browsers with the specified names.
        /// Supported browser names are defined in <see cref="BrowserNames"/> static class.
        /// </summary>
        /// <param name="browserNames">The browser names. Can be one or many of <see cref="BrowserNames" /> values.</param>
        /// <returns>The array of <see cref="DriverSetupResult"/>.</returns>
        public static DriverSetupResult[] AutoSetUp(IEnumerable<string> browserNames) =>
            AutoSetUpAsync(browserNames).GetAwaiter().GetResult();

        /// <inheritdoc cref="AutoSetUp(string)"/>
        public static async Task<DriverSetupResult> AutoSetUpAsync(string browserName) =>
            await Configure(browserName).WithAutoVersion().SetUpAsync();

        /// <inheritdoc cref="AutoSetUp(IEnumerable{string})"/>
        public static async Task<DriverSetupResult[]> AutoSetUpAsync(params string[] browserNames) =>
            await AutoSetUpAsync(browserNames?.AsEnumerable());

        /// <inheritdoc cref="AutoSetUp(IEnumerable{string})"/>
        public static async Task<DriverSetupResult[]> AutoSetUpAsync(IEnumerable<string> browserNames)
        {
            browserNames.CheckNotNull(nameof(browserNames));

            var tasks = browserNames
                .Distinct()
                .Select(AutoSetUpAsync);

            return (await Task.WhenAll(tasks))
                .Where(res => res != null)
                .ToArray();
        }

        /// <summary>
        /// Sets up drivers with auto version detection for the browsers with the specified names.
        /// Supported browser names are defined in <see cref="BrowserNames"/> static class.
        /// Skips invalid/unsupported browser names.
        /// </summary>
        /// <param name="browserNames">The browser names. Can be one or many of <see cref="BrowserNames" /> values.</param>
        /// <returns>The array of <see cref="DriverSetupResult"/>.</returns>
        public static DriverSetupResult[] AutoSetUpSafely(IEnumerable<string> browserNames) =>
            AutoSetUpSafelyAsync(browserNames).GetAwaiter().GetResult();

        /// <inheritdoc cref="AutoSetUpSafely"/>
        public static async Task<DriverSetupResult[]> AutoSetUpSafelyAsync(IEnumerable<string> browserNames) =>
            browserNames != null
                ? await AutoSetUpAsync(
                    browserNames
                        .Where(name => name != null)
                        .Distinct()
                        .Where(name => BrowserDriverSetupDataMap.ContainsKey(name)))
                : new DriverSetupResult[0];

        /// <summary>
        /// Sets up pending configurations that are stored in <see cref="PendingConfigurations" /> property.
        /// </summary>
        /// <returns>The array of <see cref="DriverSetupResult"/>.</returns>
        public static DriverSetupResult[] SetUpPendingConfigurations() =>
            SetUpPendingConfigurationsAsync().GetAwaiter().GetResult();

        /// <inheritdoc cref="SetUpPendingConfigurations"/>
        public static async Task<DriverSetupResult[]> SetUpPendingConfigurationsAsync()
        {
            var tasks = PendingConfigurations.ToArray()
                .Select(conf => conf.SetUpAsync());

            return (await Task.WhenAll(tasks))
                .Where(res => res != null)
                .ToArray();
        }

        private class DriverSetupData
        {
            public DriverSetupData(
                Func<IHttpRequestExecutor, IDriverSetupStrategy> strategyFactory,
                DriverSetupOptionsBuilder defaultOptionsBuilder)
            {
                StrategyFactory = strategyFactory;
                DefaultOptionsBuilder = defaultOptionsBuilder;
            }

            public Func<IHttpRequestExecutor, IDriverSetupStrategy> StrategyFactory { get; }

            public DriverSetupOptionsBuilder DefaultOptionsBuilder { get; }
        }
    }
}
