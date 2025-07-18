﻿namespace Atata.WebDriverSetup;

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
        "https://msedgedriver.microsoft.com";

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
    public string GetDriverBinaryFileName(TargetOSPlatform platform) =>
        platform.OSFamily == TargetOSFamily.Windows
            ? "msedgedriver.exe"
            : "msedgedriver";

    /// <inheritdoc/>
    public string GetDriverLatestVersion() =>
        _httpRequestExecutor.DownloadString(DriverLatestVersionUrl).Trim();

    /// <inheritdoc/>
    public Uri GetDriverDownloadUrl(string version, TargetOSPlatform platform) =>
        new(GetDriverDownloadUrlString(version, platform));

    private static string GetDriverDownloadUrlString(string version, TargetOSPlatform platform) =>
        GetDriverDownloadUrlVersionPart(version) + GetDriverDownloadFileName(platform);

    private static string GetDriverDownloadUrlVersionPart(string version) =>
        $"{BaseUrl}/{version}/";

    private static string GetDriverDownloadFileName(TargetOSPlatform platform) =>
        platform.OSFamily switch
        {
            TargetOSFamily.Windows => $"edgedriver_{GetWindowsArchitectureSuffix(platform.Architecture)}.zip",
            TargetOSFamily.Mac => $"edgedriver_mac64{(platform.Architecture == TargetArchitecture.Arm64 ? "_m1" : null)}.zip",
            _ => "edgedriver_linux64.zip"
        };

    private static string GetWindowsArchitectureSuffix(TargetArchitecture architecture) =>
        architecture switch
        {
            TargetArchitecture.X32 => "win32",
            TargetArchitecture.X64 => "win64",
            TargetArchitecture.Arm64 => "arm64",
            _ => throw new ArgumentException($"""Unsupported "{architecture}" architecture.""", nameof(architecture))
        };

    /// <inheritdoc/>
    public async Task<string?> GetInstalledBrowserVersionAsync(CancellationToken cancellationToken = default) =>
        OSInfo.IsWindows
            ? AppVersionDetector.GetFromProgramFiles(@"Microsoft\Edge\Application\msedge.exe")
                ?? AppVersionDetector.GetFromBLBeaconInRegistry(@"Microsoft\Edge")
                ?? AppVersionDetector.GetByApplicationPathInRegistry("msedge.exe")
            : (OSInfo.IsOSX
                ? (await AppVersionDetector.GetThroughOSXApplicationCliAsync("Microsoft Edge", cancellationToken).ConfigureAwait(false))
                : (await AppVersionDetector.GetThroughCliAsync("microsoft-edge", "--version", cancellationToken).ConfigureAwait(false)))
                    ?.Replace("Microsoft Edge ", null);

    /// <inheritdoc/>
    public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion, TargetOSPlatform platform) =>
        EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
            browserVersion,
            platform.ToOSPlatform(),
            _httpRequestExecutor,
            out string? driverVersion)
            ? driverVersion
            : browserVersion;

    /// <inheritdoc/>
    public bool TryGetDriverClosestVersion(string version, TargetOSPlatform platform, [NotNullWhen(true)] out string? closestVersion) =>
        EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(version, platform.ToOSPlatform(), _httpRequestExecutor, out closestVersion)
            || TryGetDriverClosestVersionFromDownloadsPage(version, platform, out closestVersion)
            || TryGetDriverClosestVersionFromDownloadsPage(version, platform, out closestVersion);

    private bool TryGetDriverClosestVersionFromDownloadsPage(string version, TargetOSPlatform platform, [NotNullWhen(true)] out string? closestVersion)
    {
        string originalVersionUrlVersionPart = GetDriverDownloadUrlVersionPart(version);
        string originalVersionUrlVersionHrefStart = $"href=\"{originalVersionUrlVersionPart}";

        string downloadsPageHtml = _httpRequestExecutor.DownloadString(DownloadsPage);
        int lastIndexOfOriginalVersion = downloadsPageHtml.LastIndexOf(originalVersionUrlVersionHrefStart, StringComparison.Ordinal);

        if (lastIndexOfOriginalVersion >= 0)
        {
            Regex anyVersionRegex = new($"href=\"{GetDriverDownloadUrlString("([^\"]+)", platform)}\"");

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
        Regex majorVersionRegex = new($"href=\"{GetDriverDownloadUrlString($"({majorVersion}\\.[^\"]+)", platform)}\"");
        MatchCollection majorVersionRegexMatches = majorVersionRegex.Matches(downloadsPageHtml);

        if (majorVersionRegexMatches.Count > 0)
        {
            string versionFound = majorVersionRegexMatches[^1].Groups[1].Value;

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
