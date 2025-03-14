namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a set of static methods and properties for driver configuration and setup.
/// </summary>
public static class DriverSetup
{
    private static readonly Dictionary<string, DriverSetupData> s_browserDriverSetupDataMap = [];

    private static readonly object s_pendingConfigurationsSyncLock = new();

    static DriverSetup()
    {
        RegisterStrategyFactory(BrowserNames.Chrome, hre => new ChromeDriverSetupStrategy(hre));
        RegisterStrategyFactory(BrowserNames.Firefox, hre => new FirefoxDriverSetupStrategy(hre));
        RegisterStrategyFactory(BrowserNames.Edge, hre => new EdgeDriverSetupStrategy(hre));
        RegisterStrategyFactory(BrowserNames.Opera, hre => new OperaDriverSetupStrategy(hre));
        RegisterStrategyFactory(BrowserNames.InternetExplorer, hre => new InternetExplorerDriverSetupStrategy(hre));
    }

    /// <summary>
    /// Gets the global setup options.
    /// </summary>
    public static DriverSetupOptions GlobalOptions { get; } = new();

    /// <summary>
    /// Gets the global setup configuration builder.
    /// Configures <see cref="GlobalOptions"/>.
    /// </summary>
    public static DriverSetupOptionsBuilder GlobalConfiguration { get; } = new(GlobalOptions);

    /// <summary>
    /// Gets the pending driver setup configurations,
    /// the configurations that were created but were not set up.
    /// </summary>
    public static List<DriverSetupConfigurationBuilder> PendingConfigurations { get; } = [];

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

        DriverSetupOptionsBuilder optionsBuilder = s_browserDriverSetupDataMap.TryGetValue(browserName, out DriverSetupData currentData)
            ? currentData.DefaultOptionsBuilder
            : new DriverSetupOptionsBuilder(new DriverSetupOptions(GlobalOptions));

        s_browserDriverSetupDataMap[browserName] = new DriverSetupData(driverSetupStrategyFactory, optionsBuilder);
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
        DriverSetupOptions driverSetupOptions = s_browserDriverSetupDataMap.TryGetValue(browserName, out DriverSetupData driverSetupData)
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

        lock (s_pendingConfigurationsSyncLock)
        {
            PendingConfigurations.Add(builder);
        }

        return builder;
    }

    private static DriverSetupData GetDriverSetupData(string browserName)
    {
        browserName.CheckNotNullOrWhitespace(nameof(browserName));

        return s_browserDriverSetupDataMap.TryGetValue(browserName, out DriverSetupData setupData)
            ? setupData
            : throw new ArgumentException($"""Unsupported "{browserName}" browser name.""", nameof(browserName));
    }

    private static DriverSetupConfiguration CreateConfiguration(
        string browserName,
        DriverSetupOptions driverSetupOptions) =>
        new(driverSetupOptions)
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
        await Configure(browserName).WithAutoVersion().SetUpAsync().ConfigureAwait(false);

    /// <inheritdoc cref="AutoSetUp(IEnumerable{string})"/>
    public static async Task<DriverSetupResult[]> AutoSetUpAsync(params string[] browserNames) =>
        await AutoSetUpAsync(browserNames?.AsEnumerable()).ConfigureAwait(false);

    /// <inheritdoc cref="AutoSetUp(IEnumerable{string})"/>
    public static async Task<DriverSetupResult[]> AutoSetUpAsync(IEnumerable<string> browserNames)
    {
        browserNames.CheckNotNull(nameof(browserNames));

        var tasks = browserNames
            .Distinct()
            .Select(AutoSetUpAsync);

        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        return [.. results.Where(res => res is not null)];
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
                    .Where(name => name is not null)
                    .Distinct()
                    .Where(s_browserDriverSetupDataMap.ContainsKey))
                .ConfigureAwait(false)
            : [];

    /// <summary>
    /// Sets up pending configurations that are stored in <see cref="PendingConfigurations" /> property.
    /// </summary>
    /// <returns>The array of <see cref="DriverSetupResult"/>.</returns>
    public static DriverSetupResult[] SetUpPendingConfigurations() =>
        SetUpPendingConfigurationsAsync().GetAwaiter().GetResult();

    /// <inheritdoc cref="SetUpPendingConfigurations"/>
    public static async Task<DriverSetupResult[]> SetUpPendingConfigurationsAsync()
    {
        DriverSetupConfigurationBuilder[] pendingConfigurations;

        lock (s_pendingConfigurationsSyncLock)
        {
            pendingConfigurations = [.. PendingConfigurations];
        }

        var tasks = pendingConfigurations.Select(conf => conf.SetUpAsync());

        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        return [.. results.Where(res => res is not null)];
    }

    internal static void RemovePendingConfiguration(DriverSetupConfigurationBuilder configurationBuilder)
    {
        lock (s_pendingConfigurationsSyncLock)
        {
            PendingConfigurations.Remove(configurationBuilder);
        }
    }

    internal static string GetInstalledBrowserVersion(string browserName) =>
        s_browserDriverSetupDataMap.TryGetValue(browserName, out var driverSetupData)
            ? (driverSetupData.StrategyFactory.Invoke(new HttpRequestExecutor()) as IGetsInstalledBrowserVersion)
                ?.GetInstalledBrowserVersion()
            : null;

    private sealed class DriverSetupData
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
