namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the specific driver setup configuration.
/// </summary>
public class DriverSetupConfiguration : DriverSetupOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DriverSetupConfiguration"/> class using <paramref name="baseOptions"/>.
    /// </summary>
    /// <param name="baseOptions">The base options.</param>
    public DriverSetupConfiguration(DriverSetupOptions baseOptions)
        : base(baseOptions)
    {
        if (baseOptions is not DriverSetupConfiguration)
            Version = DriverVersions.Auto;
    }

    /// <summary>
    /// Gets or sets the version.
    /// The default value is <see cref="DriverVersions.Auto"/>.
    /// </summary>
    public string Version
    {
        get => GetOption<string>(nameof(Version));
        set => SetOption(nameof(Version), value);
    }

    /// <summary>
    /// <para>
    /// Gets or sets the name of the environment variable
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
    public string EnvironmentVariableName
    {
        get => GetOption<string>(nameof(EnvironmentVariableName));
        set => SetOption(nameof(EnvironmentVariableName), value);
    }
}
