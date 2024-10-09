namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the Edge driver (<c>msedgedriver.exe</c>/<c>msedgedriver</c>) setup strategy.
/// </summary>
public class EdgeDriverSetupStrategy :
    IDriverSetupStrategy,
    IGetsDriverLatestVersion,
    IGetsInstalledBrowserVersion,
    IGetsDriverVersionCorrespondingToBrowserVersion,
    IGetsDriverClosestVersion
{
    private const string BaseUrl =
        "https://msedgedriver.azureedge.net";

    private const string DriverLatestVersionUrl =
        BaseUrl + "/LATEST_STABLE";

    private const string DownloadsPage = "https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/";

    private readonly IHttpRequestExecutor _httpRequestExecutor;

    /// <summary>
    /// Initializes a new instance of the <see cref="EdgeDriverSetupStrategy"/> class.
    /// </summary>
    /// <param name="httpRequestExecutor">The HTTP request executor.</param>
    public EdgeDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor) =>
        _httpRequestExecutor = httpRequestExecutor;

    /// <inheritdoc/>
    public string DriverBinaryFileName { get; } =
        OSInfo.IsWindows
            ? "msedgedriver.exe"
            : "msedgedriver";

    /// <inheritdoc/>
    public string GetDriverLatestVersion() =>
        _httpRequestExecutor.DownloadString(DriverLatestVersionUrl).Trim();

    /// <inheritdoc/>
    public Uri GetDriverDownloadUrl(string version, Architecture architecture) =>
        new(GetDriverDownloadUrlString(version, architecture));

    private static string GetDriverDownloadUrlString(string version, Architecture architecture) =>
        GetDriverDownloadUrlVersionPart(version) + GetDriverDownloadFileName(architecture);

    private static string GetDriverDownloadUrlVersionPart(string version) =>
        $"{BaseUrl}/{version}/";

    private static string GetDriverDownloadFileName(Architecture architecture) =>
        OSInfo.IsWindows
            ? $"edgedriver_{GetWindowsArchitectureSuffix(architecture)}.zip"
            : OSInfo.IsOSX
                ? $"edgedriver_mac64{(architecture == Architecture.Arm64 ? "_m1" : null)}.zip"
                : "edgedriver_linux64.zip";

    private static string GetWindowsArchitectureSuffix(Architecture architecture) =>
        architecture switch
        {
            Architecture.X32 => "win32",
            Architecture.X64 => "win64",
            Architecture.Arm64 => "arm64",
            _ => throw new ArgumentException($"""Unsupported "{architecture}" architecture.""", nameof(architecture))
        };

    /// <inheritdoc/>
    public string GetInstalledBrowserVersion() =>
        OSInfo.IsWindows
            ? AppVersionDetector.GetFromProgramFiles(@"Microsoft\Edge\Application\msedge.exe")
                ?? AppVersionDetector.GetFromBLBeaconInRegistry(@"Microsoft\Edge")
                ?? AppVersionDetector.GetByApplicationPathInRegistry("msedge.exe")
            : (OSInfo.IsOSX
                ? AppVersionDetector.GetThroughOSXApplicationCli("Microsoft Edge")
                : AppVersionDetector.GetThroughCli("microsoft-edge", "--version"))
                ?.Replace("Microsoft Edge ", null);

    /// <inheritdoc/>
    public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion) =>
        browserVersion;

    /// <inheritdoc/>
    public bool TryGetDriverClosestVersion(string version, Architecture architecture, out string closestVersion) =>
        TryAttemptToGetDriverClosestVersion(version, architecture, out closestVersion)
            || TryAttemptToGetDriverClosestVersion(version, architecture, out closestVersion);

    private bool TryAttemptToGetDriverClosestVersion(string version, Architecture architecture, out string closestVersion)
    {
        string originalVersionUrlVersionPart = GetDriverDownloadUrlVersionPart(version);
        string originalVersionUrlVersionHrefStart = $"href=\"{originalVersionUrlVersionPart}";

        string downloadsPageHtml = _httpRequestExecutor.DownloadString(DownloadsPage);
        int lastIndexOfOriginalVersion = downloadsPageHtml.LastIndexOf(originalVersionUrlVersionHrefStart, StringComparison.Ordinal);

        if (lastIndexOfOriginalVersion >= 0)
        {
            Regex anyVersionRegex = new($"href=\"{GetDriverDownloadUrlString("([^\"]+)", architecture)}\"");

            Match previousVersionRegexMatch = anyVersionRegex.Match(downloadsPageHtml, lastIndexOfOriginalVersion + originalVersionUrlVersionHrefStart.Length);

            if (previousVersionRegexMatch.Success)
            {
                string versionFound = previousVersionRegexMatch.Groups[1].Value;

                if (versionFound != version)
                {
                    closestVersion = versionFound;
                    return true;
                }
            }
        }

        string majorVersion = VersionUtils.TrimMinor(version);
        Regex majorVersionRegex = new($"href=\"{GetDriverDownloadUrlString($"({majorVersion}\\.[^\"]+)", architecture)}\"");
        MatchCollection majorVersionRegexMatches = majorVersionRegex.Matches(downloadsPageHtml);

        if (majorVersionRegexMatches.Count > 0)
        {
            string versionFound = majorVersionRegexMatches[majorVersionRegexMatches.Count - 1].Groups[1].Value;

            if (versionFound != version)
            {
                closestVersion = versionFound;
                return true;
            }
        }

        closestVersion = null;
        return false;
    }
}
