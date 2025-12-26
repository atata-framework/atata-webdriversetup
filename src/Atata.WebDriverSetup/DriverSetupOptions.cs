namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the common driver setup options.
/// </summary>
public class DriverSetupOptions
{
    private readonly DriverSetupOptions? _baseOptions;

    private readonly Dictionary<string, object?> _optionValues = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="DriverSetupOptions"/> class.
    /// </summary>
    /// <param name="baseOptions">The base options.</param>
    public DriverSetupOptions(DriverSetupOptions? baseOptions = null)
    {
        _baseOptions = baseOptions;

        if (baseOptions is null)
        {
            StorageDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "drivers");
#if !NETFRAMEWORK
            CheckCertificateRevocationList = true;
#endif
            UseInterProcessSynchronization = false;
            UseVersionCache = true;

            LatestVersionCheckInterval = TimeSpan.FromHours(2);
            SpecificVersionCheckInterval = TimeSpan.FromHours(2);

            HttpRequestTryCount = 3;
            HttpRequestRetryInterval = TimeSpan.FromSeconds(3);

            IsEnabled = true;
            AddToEnvironmentPathVariable = true;
        }
    }

    /// <summary>
    /// Gets or sets the storage directory path.
    /// The default value is <c>"{basedir}/drivers")</c>.
    /// </summary>
    public string StorageDirectoryPath
    {
        get => GetOption<string>(nameof(StorageDirectoryPath))!;
        set => SetOption(nameof(StorageDirectoryPath), value);
    }

    /// <summary>
    /// Gets or sets the architecture (auto, x32 or x64).
    /// The default value is <see cref="Architecture.Auto"/>.
    /// </summary>
    public Architecture Architecture
    {
        get => GetOption<Architecture>(nameof(Architecture));
        set => SetOption(nameof(Architecture), value);
    }

    /// <summary>
    /// Gets or sets the proxy.
    /// </summary>
    public IWebProxy? Proxy
    {
        get => GetOption<IWebProxy>(nameof(Proxy));
        set => SetOption(nameof(Proxy), value);
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Gets or sets a value indicating whether the certificate is automatically picked
    /// from the certificate store or if the caller is allowed to pass in a specific
    /// client certificate.
    /// The default value is <see langword="true"/>.
    /// </summary>
    public bool CheckCertificateRevocationList
    {
        get => GetOption<bool>(nameof(CheckCertificateRevocationList));
        set => SetOption(nameof(CheckCertificateRevocationList), value);
    }
#endif

    /// <summary>
    /// Gets or sets the configuration action of <see cref="HttpClientHandler"/>.
    /// The <see cref="HttpClientHandler"/> instance is used to get a driver version information
    /// and to download a driver archive.
    /// </summary>
    /// <value>
    /// The HTTP client handler configuration action.
    /// </value>
    public Action<HttpClientHandler>? HttpClientHandlerConfigurationAction
    {
        get => GetOption<Action<HttpClientHandler>>(nameof(HttpClientHandlerConfigurationAction));
        set => SetOption(nameof(HttpClientHandlerConfigurationAction), value);
    }

    [Obsolete("Use UseInterProcessSynchronization instead.")]
    public bool UseMutex
    {
        get => UseInterProcessSynchronization;
        set => UseInterProcessSynchronization = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use inter-process synchronization to synchronize driver setup across machine.
    /// The default value is <see langword="false"/>.
    /// </summary>
    public bool UseInterProcessSynchronization
    {
        get => GetOption<bool>(nameof(UseInterProcessSynchronization));
        set => SetOption(nameof(UseInterProcessSynchronization), value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use version cache.
    /// The default value is <see langword="true"/>.
    /// </summary>
    public bool UseVersionCache
    {
        get => GetOption<bool>(nameof(UseVersionCache));
        set => SetOption(nameof(UseVersionCache), value);
    }

    /// <summary>
    /// Gets or sets the latest version check interval.
    /// The default value is <c>2</c> hours.
    /// </summary>
    public TimeSpan LatestVersionCheckInterval
    {
        get => GetOption<TimeSpan>(nameof(LatestVersionCheckInterval));
        set => SetOption(nameof(LatestVersionCheckInterval), value);
    }

    /// <summary>
    /// Gets or sets the specific version check interval.
    /// The default value is <c>2</c> hours.
    /// </summary>
    public TimeSpan SpecificVersionCheckInterval
    {
        get => GetOption<TimeSpan>(nameof(SpecificVersionCheckInterval));
        set => SetOption(nameof(SpecificVersionCheckInterval), value);
    }

    /// <summary>
    /// Gets or sets the HTTP request try count.
    /// The default value is <c>3</c>.
    /// </summary>
    public int HttpRequestTryCount
    {
        get => GetOption<int>(nameof(HttpRequestTryCount));
        set => SetOption(nameof(HttpRequestTryCount), value);
    }

    /// <summary>
    /// Gets or sets the HTTP request retry interval.
    /// The default value is <c>3</c> seconds.
    /// </summary>
    public TimeSpan HttpRequestRetryInterval
    {
        get => GetOption<TimeSpan>(nameof(HttpRequestRetryInterval));
        set => SetOption(nameof(HttpRequestRetryInterval), value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is enabled.
    /// The default value is <see langword="true"/>.
    /// </summary>
    public bool IsEnabled
    {
        get => GetOption<bool>(nameof(IsEnabled));
        set => SetOption(nameof(IsEnabled), value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to add the driver directory path
    /// to environment "Path" variable.
    /// The default value is <see langword="true"/>.
    /// </summary>
    public bool AddToEnvironmentPathVariable
    {
        get => GetOption<bool>(nameof(AddToEnvironmentPathVariable));
        set => SetOption(nameof(AddToEnvironmentPathVariable), value);
    }

    /// <summary>
    /// Gets the option value from this options or a base one.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="optionName">Name of the option.</param>
    /// <returns>The option value or default value.</returns>
    protected T? GetOption<T>(string optionName) =>
        _optionValues.TryGetValue(optionName, out object? value)
            ? (T)value!
            : _baseOptions is not null
                ? _baseOptions.GetOption<T>(optionName)
                : default;

    /// <summary>
    /// Sets the option value.
    /// </summary>
    /// <param name="optionName">Name of the option.</param>
    /// <param name="value">The value to set.</param>
    protected void SetOption(string optionName, object? value) =>
        _optionValues[optionName] = value;

    internal Action<HttpClientHandler>? CreateAggregateHttpClientHandlerConfigurationAction()
    {
        var resultAction = HttpClientHandlerConfigurationAction;

        if (Proxy is not null)
        {
            Action<HttpClientHandler> setProxyAction = x => x.Proxy = Proxy;
            resultAction = resultAction is null
                ? setProxyAction
                : setProxyAction + resultAction;
        }

#if !NETFRAMEWORK
        if (CheckCertificateRevocationList)
        {
            Action<HttpClientHandler> checkCertificateAction = x => x.CheckCertificateRevocationList = true;
            resultAction = resultAction is null
                ? checkCertificateAction
                : checkCertificateAction + resultAction;
        }
#endif

        return resultAction;
    }
}
