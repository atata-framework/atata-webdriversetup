﻿namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the driver setup configuration builder.
/// </summary>
public class DriverSetupConfigurationBuilder : DriverSetupOptionsBuilder<DriverSetupConfigurationBuilder, DriverSetupConfiguration>
{
    private readonly Func<IHttpRequestExecutor, IDriverSetupStrategy> _driverSetupStrategyFactory;

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
        _driverSetupStrategyFactory = driverSetupStrategyFactory.CheckNotNull(nameof(driverSetupStrategyFactory));
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
    public DriverSetupResult SetUp()
    {
        if (BuildingContext.IsEnabled)
        {
            return BuildingContext.UseMutex
                ? ExecuteSetUpUsingMutex()
                : ExecuteSetUp();
        }
        else
        {
            return null;
        }
    }

    private DriverSetupResult ExecuteSetUpUsingMutex()
    {
        const int timeoutToWait = 600_000;
        string mutexId = $@"Global\{{50E4E9F8-971F-440E-B7BE-D4B584350529}}-{BrowserName.ToLowerInvariant()}";

        using var mutex = new Mutex(false, mutexId, out _);
        var hasHandle = false;

        try
        {
            try
            {
                hasHandle = mutex.WaitOne(timeoutToWait, false);

                if (!hasHandle)
                    throw new TimeoutException("Timeout waiting for driver setup mutex.");
            }
            catch (AbandonedMutexException)
            {
                hasHandle = true;
            }

            return ExecuteSetUp();
        }
        finally
        {
            if (hasHandle)
                mutex.ReleaseMutex();
        }
    }

    private DriverSetupResult ExecuteSetUp()
    {
        IHttpRequestExecutor httpRequestExecutor = CreateHttpRequestExecutor();

        IDriverSetupStrategy setupStrategy = _driverSetupStrategyFactory.Invoke(httpRequestExecutor);

        string driverVersion = ResolveDriverVersion(setupStrategy, BuildingContext.Version);

        DriverSetupExecutor setupExecutor = new DriverSetupExecutor(
            BrowserName,
            BuildingContext,
            setupStrategy,
            httpRequestExecutor);

        DriverSetupResult result = setupExecutor.SetUp(driverVersion);

        DriverSetup.RemovePendingConfiguration(this);

        return result;
    }

    /// <inheritdoc cref="SetUp"/>
    public async Task<DriverSetupResult> SetUpAsync() =>
        await Task.Run(SetUp).ConfigureAwait(false);

    private IHttpRequestExecutor CreateHttpRequestExecutor() =>
        new ReliableHttpRequestExecutor(
            new HttpRequestExecutor(
                BuildingContext.Proxy,
                BuildingContext.CheckCertificateRevocationList,
                BuildingContext.HttpClientHandlerConfigurationAction),
            BuildingContext.HttpRequestTryCount,
            BuildingContext.HttpRequestRetryInterval);

    private string ResolveDriverVersion(IDriverSetupStrategy setupStrategy, string version)
    {
        DriverVersionResolver driverVersionResolver = new DriverVersionResolver(
            BrowserName, BuildingContext, setupStrategy);

        if (version == DriverVersions.Auto)
            return driverVersionResolver.ResolveCorrespondingOrLatestVersion();
        else if (version == DriverVersions.Latest)
            return driverVersionResolver.ResolveLatestVersion();
        else if (DriverVersions.TryExtractBrowserVersion(version, out string browserVersion))
            return driverVersionResolver.ResolveByBrowserVersion(browserVersion);
        else
            return version;
    }
}
