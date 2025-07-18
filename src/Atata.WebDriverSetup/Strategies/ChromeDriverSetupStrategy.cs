﻿namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the Chrome driver (<c>chromedriver.exe</c>/<c>chromedriver</c>) setup strategy.
/// </summary>
public class ChromeDriverSetupStrategy :
    IDriverSetupStrategy,
    IGetsDriverLatestVersion,
    IGetsInstalledBrowserVersion,
    IGetsDriverVersionCorrespondingToBrowserVersion
{
    private const string CftApiBaseUrl =
        "https://googlechromelabs.github.io/chrome-for-testing";

    private const string Cft1DownloadsBaseUrl =
        "https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing";

    private const string Cft2DownloadsBaseUrl =
        "https://storage.googleapis.com/chrome-for-testing-public";

    private const string OldBaseUrl =
        "https://chromedriver.storage.googleapis.com";

    private const string OldDriverLatestVersionUrl =
        OldBaseUrl + "/LATEST_RELEASE";

    private const string OldDriverSpecificVersionUrlFormat =
        OldDriverLatestVersionUrl + "_{0}";

    private const int Cft1StartingVersionMajorNumber = 115;

    private static readonly Version s_cft2StartingVersion = new(121, 0, 6167, 85);

    private readonly IHttpRequestExecutor _httpRequestExecutor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChromeDriverSetupStrategy"/> class.
    /// </summary>
    /// <param name="httpRequestExecutor">The HTTP request executor.</param>
    public ChromeDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor) =>
        _httpRequestExecutor = httpRequestExecutor;

    /// <inheritdoc/>
    public string GetDriverBinaryFileName(TargetOSPlatform platform) =>
        platform.OSFamily == TargetOSFamily.Windows
            ? "chromedriver.exe"
            : "chromedriver";

    /// <inheritdoc/>
    public string GetDriverLatestVersion()
    {
        using Stream versionsStream = _httpRequestExecutor.DownloadStream(CftApiBaseUrl + "/last-known-good-versions.json");
        using JsonDocument jsonDocument = JsonDocument.Parse(versionsStream);

        return jsonDocument.RootElement
            .GetPropertyByChain("channels", "Stable", "version")
            .GetString()!;
    }

    /// <inheritdoc/>
    public Uri GetDriverDownloadUrl(string version, TargetOSPlatform platform)
    {
        if (VersionUtils.GetNumbersCount(version) != 4)
            throw new ArgumentException($"Invalid driver \"{version}\" version. The version should consist of 4 numbers in format \"115.0.0.0\".", nameof(version));

        if (IsCftVersion(version))
        {
            string baseUrl = GetCftDownloadsBaseUrl(version);
            string platformName = GetCftDriverDownloadPlatformName(platform);
            return new Uri($"{baseUrl}/{version}/{platformName}/chromedriver-{platformName}.zip");
        }
        else
        {
            return new Uri($"{OldBaseUrl}/{version}/{GetOldDriverDownloadFileName(platform)}");
        }
    }

    private static bool IsCftVersion(string version) =>
        VersionUtils.GetMajorNumber(version) >= Cft1StartingVersionMajorNumber;

    private static string GetCftDownloadsBaseUrl(string version)
    {
        Version parsedVersion;

        try
        {
            parsedVersion = VersionUtils.Parse(version);
        }
        catch
        {
            // In case of possible future version number style change.
            return Cft2DownloadsBaseUrl;
        }

        return parsedVersion < s_cft2StartingVersion
            ? Cft1DownloadsBaseUrl
            : Cft2DownloadsBaseUrl;
    }

    private static string GetOldDriverDownloadFileName(TargetOSPlatform platform) =>
        platform.OSFamily switch
        {
            TargetOSFamily.Windows => "chromedriver_win32.zip",
            TargetOSFamily.Mac => $"chromedriver_mac{(platform.Architecture == TargetArchitecture.Arm64 ? "_arm" : null)}64.zip",
            _ => "chromedriver_linux64.zip"
        };

    private static string GetCftDriverDownloadPlatformName(TargetOSPlatform platform) =>
        platform.OSFamily switch
        {
            TargetOSFamily.Windows => $"win{platform.Bits}",
            TargetOSFamily.Mac => $"mac-{(platform.Architecture == TargetArchitecture.Arm64 ? "arm" : "x")}64",
            _ => "linux64"
        };

    /// <inheritdoc/>
    public async Task<string?> GetInstalledBrowserVersionAsync(CancellationToken cancellationToken = default) =>
        OSInfo.IsWindows
            ? AppVersionDetector.GetFromProgramFiles(@"Google\Chrome\Application\chrome.exe")
                ?? AppVersionDetector.GetFromBLBeaconInRegistry(@"Google\Chrome")
                ?? AppVersionDetector.GetByApplicationPathInRegistry("chrome.exe")
            : OSInfo.IsOSX
                ? (await AppVersionDetector.GetThroughOSXApplicationCliAsync("Google Chrome", cancellationToken).ConfigureAwait(false))
                    ?.Replace("Google Chrome ", null)
                : (await AppVersionDetector.GetThroughCliAsync("google-chrome", "--product-version", cancellationToken).ConfigureAwait(false));

    /// <inheritdoc/>
    public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion, TargetOSPlatform platform)
    {
        Guard.ThrowIfNullOrWhitespace(browserVersion);

        int browserVersionNumbersCount = VersionUtils.GetNumbersCount(browserVersion);

        string browserVersionToUse = browserVersionNumbersCount switch
        {
            1 => browserVersion,
            2 => VersionUtils.TrimMinor(browserVersion),
            _ => VersionUtils.TrimRevision(browserVersion)
        };

        if (IsCftVersion(browserVersionToUse))
        {
            return browserVersionNumbersCount <= 2
                ? GetDriverVersionFromCftEndpoint("/latest-versions-per-milestone.json", "milestones", browserVersionToUse)
                : GetDriverVersionFromCftEndpoint("/latest-patch-versions-per-build.json", "builds", browserVersionToUse);
        }
        else
        {
            string url = string.Format(OldDriverSpecificVersionUrlFormat, browserVersionToUse);

            return _httpRequestExecutor.DownloadString(url);
        }
    }

    private string GetDriverVersionFromCftEndpoint(string relativeEndpoint, string subRootPropertyName, string incompleteBrowserVersion)
    {
        using Stream versionsStream = _httpRequestExecutor.DownloadStream(CftApiBaseUrl + relativeEndpoint);
        using JsonDocument jsonDocument = JsonDocument.Parse(versionsStream);

        return jsonDocument.RootElement
            .GetPropertyByChain(subRootPropertyName, incompleteBrowserVersion, "version")
            .GetString()!;
    }
}
