namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the Firefox/Gecko driver (<c>geckodriver.exe</c>/<c>geckodriver</c>) setup strategy.
/// </summary>
public class FirefoxDriverSetupStrategy :
    GitHubRepositoryBasedDriverSetupStrategy,
    IGetsInstalledBrowserVersion,
    IGetsDriverVersionCorrespondingToBrowserVersion
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirefoxDriverSetupStrategy"/> class.
    /// </summary>
    /// <param name="httpRequestExecutor">The HTTP request executor.</param>
    public FirefoxDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
        : base(httpRequestExecutor, "mozilla", "geckodriver")
    {
    }

    /// <inheritdoc/>
    public override string GetDriverBinaryFileName(TargetOSPlatform platform) =>
        platform.OSFamily == TargetOSFamily.Windows
            ? "geckodriver.exe"
            : "geckodriver";

    /// <inheritdoc/>
    protected override string GetDriverDownloadFileName(string version, TargetOSPlatform platform)
    {
        string commonNamePart = $"geckodriver-v{version}-";

        return platform.OSFamily switch
        {
            TargetOSFamily.Windows => $"{commonNamePart}win{GetArchitectureSuffix(platform.Architecture)}.zip",
            TargetOSFamily.Mac => $"{commonNamePart}macos{(platform.Architecture == TargetArchitecture.Arm64 ? GetArchitectureSuffix(TargetArchitecture.Arm64) : null)}.tar.gz",
            _ => $"{commonNamePart}linux{GetArchitectureSuffix(platform.Architecture)}.tar.gz"
        };
    }

    private static string GetArchitectureSuffix(TargetArchitecture architecture) =>
        architecture switch
        {
            TargetArchitecture.X32 => "32",
            TargetArchitecture.X64 => "64",
            TargetArchitecture.Arm64 => "-aarch64",
            _ => throw new ArgumentException($"""Unsupported "{architecture}" architecture.""", nameof(architecture))
        };

    /// <inheritdoc/>
    public async Task<string?> GetInstalledBrowserVersionAsync(CancellationToken cancellationToken = default) =>
        OSInfo.IsWindows
            ? AppVersionDetector.GetFromProgramFiles(@"Mozilla Firefox\firefox.exe")
                ?? RegistryUtils.GetValue(@"HKEY_CURRENT_USER\Software\Mozilla\Mozilla Firefox")
                ?? AppVersionDetector.GetByApplicationPathInRegistry("firefox.exe")
            : (OSInfo.IsOSX
                ? (await AppVersionDetector.GetThroughOSXApplicationCliAsync("Firefox", cancellationToken).ConfigureAwait(false))
                : (await AppVersionDetector.GetThroughCliAsync("firefox", "-v", cancellationToken).ConfigureAwait(false)))
                ?.Replace("Mozilla Firefox ", null);

    /// <inheritdoc/>
    public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion, TargetOSPlatform platform)
    {
        Guard.ThrowIfNullOrWhitespace(browserVersion);

        string browserMajorVersion = VersionUtils.TrimMinor(browserVersion);
        int browserMajorVersionNumber = int.Parse(browserMajorVersion);

        return browserMajorVersionNumber >= 91
            ? "0.31.0"
            : browserMajorVersionNumber >= 78
                ? "0.30.0"
                : "0.29.1";
    }
}
