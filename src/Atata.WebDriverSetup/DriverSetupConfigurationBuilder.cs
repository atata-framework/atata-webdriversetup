namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the driver setup configuration builder.
/// </summary>
public class DriverSetupConfigurationBuilder : DriverSetupOptionsBuilder<DriverSetupConfigurationBuilder, DriverSetupConfiguration>
{
    private readonly Func<IHttpRequestExecutor, IDriverSetupStrategy> _driverSetupStrategyFactory;

    private Func<DriverSetupConfiguration, IHttpRequestExecutor> _httpRequestExecutorFactory = CreateDefaultHttpRequestExecutor;

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
        Guard.ThrowIfNull(browserName);
        Guard.ThrowIfNull(driverSetupStrategyFactory);

        BrowserName = browserName;
        _driverSetupStrategyFactory = driverSetupStrategyFactory;
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
        Guard.ThrowIfNullOrWhitespace(version);
        BuildingContext.Version = version;
        return this;
    }

    /// <summary>
    /// <para>
    /// Sets the name of the environment variable
    /// that will be set with a value equal to the driver directory path.
    /// </para>
    /// <para>
    /// The default value is specific to the driver being configured.
    /// It has <c>"{BrowserName}Driver"</c> format.
    /// For example: <c>"ChromeDriver"</c> or <c>"InternetExplorerDriver"</c>.
    /// </para>
    /// <para>
    /// The <see langword="null"/> value means that none variable should be set.
    /// </para>
    /// </summary>
    /// <param name="variableName">The variable name.</param>
    /// <returns>The same builder instance.</returns>
    public DriverSetupConfigurationBuilder WithEnvironmentVariableName(string variableName)
    {
        BuildingContext.EnvironmentVariableName = variableName;
        return this;
    }

    /// <summary>
    /// Sets up driver.
    /// </summary>
    /// <returns>
    /// The <see cref="DriverSetupResult"/> instance;
    /// or <see langword="null"/>, if the configuration
    /// <see cref="DriverSetupOptions.IsEnabled"/> property is <see langword="false"/>.
    /// </returns>
    public DriverSetupResult SetUp() =>
        SetUpAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Sets up driver.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The <see cref="DriverSetupResult"/> instance;
    /// or <see langword="null"/>, if the configuration
    /// <see cref="DriverSetupOptions.IsEnabled"/> property is <see langword="false"/>.
    /// </returns>
    public async Task<DriverSetupResult> SetUpAsync(CancellationToken cancellationToken = default)
    {
        if (BuildingContext.IsEnabled)
        {
            return BuildingContext.UseInterProcessSynchronization
                ? (await ExecuteSetUpUsingInterProcessSynchronizationAsync(cancellationToken).ConfigureAwait(false))
                : (await ExecuteSetUpAsync(cancellationToken).ConfigureAwait(false));
        }
        else
        {
            return null!;
        }
    }

    internal DriverSetupConfigurationBuilder WithHttpRequestExecutor(
        Func<DriverSetupConfiguration, IHttpRequestExecutor> httpRequestExecutorFactory)
    {
        Guard.ThrowIfNull(httpRequestExecutorFactory);

        _httpRequestExecutorFactory = httpRequestExecutorFactory;
        return this;
    }

    private async Task<DriverSetupResult> ExecuteSetUpUsingInterProcessSynchronizationAsync(CancellationToken cancellationToken)
    {
        TimeSpan timeoutToWait = TimeSpan.FromMinutes(3);

        string lockFilePath = Path.Combine(Path.GetTempPath(), $"Atata.WebDriverSetup-{BrowserName.ToLowerInvariant()}.lock");
        using AsyncFileLock fileLock = new(lockFilePath);

        bool hasLock = await fileLock.WaitAsync(timeoutToWait, cancellationToken)
            .ConfigureAwait(false);

        if (!hasLock)
            throw new TimeoutException($"Timeout waiting for driver setup inter-process synchronization lock file \"{lockFilePath}\".");

        return await ExecuteSetUpAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<DriverSetupResult> ExecuteSetUpAsync(CancellationToken cancellationToken)
    {
        IHttpRequestExecutor httpRequestExecutor = _httpRequestExecutorFactory.Invoke(BuildingContext);

        IDriverSetupStrategy setupStrategy = _driverSetupStrategyFactory.Invoke(httpRequestExecutor);

        DriverVersionResolver driverVersionResolver = new(BrowserName, BuildingContext, setupStrategy);

        string driverVersion = await ResolveDriverVersionAsync(driverVersionResolver, cancellationToken)
            .ConfigureAwait(false);

        DriverSetupExecutor setupExecutor = new(
            BrowserName,
            BuildingContext,
            setupStrategy,
            httpRequestExecutor);

        DriverSetupResult result = await SetUpDriverAsync(driverVersionResolver, setupExecutor, driverVersion, cancellationToken)
            .ConfigureAwait(false);

        DriverSetup.RemovePendingConfiguration(this);

        return result;
    }

    private async Task<DriverSetupResult> SetUpDriverAsync(
        DriverVersionResolver driverVersionResolver,
        DriverSetupExecutor setupExecutor,
        string driverVersion,
        CancellationToken cancellationToken)
    {
        try
        {
            return await setupExecutor.SetUpAsync(driverVersion, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e) when (e.InnerException is HttpRequestException)
        {
            if (BuildingContext.Version is DriverVersions.Auto or DriverVersions.Latest)
            {
                string? closestDriverVersion = await driverVersionResolver.ResolveClosestVersionAsync(driverVersion, cancellationToken)
                    .ConfigureAwait(false);

                if (closestDriverVersion is not null)
                {
                    try
                    {
                        return await setupExecutor.SetUpAsync(closestDriverVersion, cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        Log.Warn(exception, $"Failed to set-up driver with version {closestDriverVersion} closest to {driverVersion}.");
                    }
                }
            }

            throw;
        }
    }

    internal static IHttpRequestExecutor CreateDefaultHttpRequestExecutor(DriverSetupConfiguration configuration)
    {
        Action<HttpClientHandler>? handlerConfigurationAction = configuration.CreateAggregateHttpClientHandlerConfigurationAction();

        IHttpRequestExecutor executor = new HttpRequestExecutor(handlerConfigurationAction);

#if DEBUG
        executor = new LoggingHttpRequestExecutor(executor);
#endif

        return new ReliableHttpRequestExecutor(
            executor,
            configuration.HttpRequestTryCount,
            configuration.HttpRequestRetryInterval);
    }

    private async Task<string> ResolveDriverVersionAsync(DriverVersionResolver driverVersionResolver, CancellationToken cancellationToken)
    {
        string version = BuildingContext.Version;

        if (version == DriverVersions.Auto)
            return await driverVersionResolver.ResolveCorrespondingOrLatestVersionAsync(cancellationToken).ConfigureAwait(false);
        else if (version == DriverVersions.Latest)
            return await driverVersionResolver.ResolveLatestVersionAsync(cancellationToken).ConfigureAwait(false);
        else if (DriverVersions.TryExtractBrowserVersion(version, out string? browserVersion))
            return await driverVersionResolver.ResolveByBrowserVersionAsync(browserVersion, cancellationToken).ConfigureAwait(false);
        else
            return version;
    }
}
