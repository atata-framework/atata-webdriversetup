using System;
using System.IO;
using System.Text.Json;

namespace Atata.WebDriverSetup
{
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

        private const string CftDownloadsBaseUrl =
            "https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing";

        private const string OldBaseUrl =
            "https://chromedriver.storage.googleapis.com";

        private const string OldDriverLatestVersionUrl =
            OldBaseUrl + "/LATEST_RELEASE";

        private const string OldDriverSpecificVersionUrlFormat =
            OldDriverLatestVersionUrl + "_{0}";

        private const int CftStartingVersionNumber = 115;

        private readonly IHttpRequestExecutor _httpRequestExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeDriverSetupStrategy"/> class.
        /// </summary>
        /// <param name="httpRequestExecutor">The HTTP request executor.</param>
        public ChromeDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor) =>
            _httpRequestExecutor = httpRequestExecutor;

        /// <inheritdoc/>
        public string DriverBinaryFileName { get; } =
            OSInfo.IsWindows
                ? "chromedriver.exe"
                : "chromedriver";

        /// <inheritdoc/>
        public string GetDriverLatestVersion()
        {
            using Stream versionsStream = _httpRequestExecutor.DownloadStream(CftApiBaseUrl + "/last-known-good-versions.json");
            using JsonDocument jsonDocument = JsonDocument.Parse(versionsStream);

            return jsonDocument.RootElement
                .GetPropertyByChain("channels", "Stable", "version")
                .GetString();
        }

        /// <inheritdoc/>
        public Uri GetDriverDownloadUrl(string version, Architecture architecture)
        {
            if (VersionUtils.GetNumbersCount(version) != 4)
                throw new ArgumentException($"Invalid driver \"{version}\" version. The version should consist of 4 numbers in format \"115.0.0.0\".", nameof(version));

            if (IsCftVersion(version))
            {
                string platform = GetCftDriverDownloadPlatform(architecture);
                return new Uri($"{CftDownloadsBaseUrl}/{version}/{platform}/chromedriver-{platform}.zip");
            }
            else
            {
                return new Uri($"{OldBaseUrl}/{version}/{GetOldDriverDownloadFileName(architecture)}");
            }
        }

        private static bool IsCftVersion(string version) =>
            VersionUtils.GetMajorNumber(version) >= CftStartingVersionNumber;

        private static string GetOldDriverDownloadFileName(Architecture architecture) =>
            OSInfo.IsWindows
                ? "chromedriver_win32.zip"
                : OSInfo.IsOSX
                    ? $"chromedriver_mac{(architecture == Architecture.Arm64 ? "_arm" : null)}64.zip"
                    : "chromedriver_linux64.zip";

        private static string GetCftDriverDownloadPlatform(Architecture architecture) =>
            OSInfo.IsWindows
                ? $"win{architecture.GetBits()}"
                : OSInfo.IsOSX
                    ? $"mac-{(architecture == Architecture.Arm64 ? "arm" : "x")}64"
                    : "linux64";

        /// <inheritdoc/>
        public string GetInstalledBrowserVersion() =>
            OSInfo.IsWindows
                ? AppVersionDetector.GetFromProgramFiles(@"Google\Chrome\Application\chrome.exe")
                    ?? AppVersionDetector.GetFromBLBeaconInRegistry(@"Google\Chrome")
                    ?? AppVersionDetector.GetByApplicationPathInRegistry("chrome.exe")
                : OSInfo.IsOSX
                    ? AppVersionDetector.GetThroughOSXApplicationCli("Google Chrome")
                        ?.Replace("Google Chrome ", null)
                    : AppVersionDetector.GetThroughCli("google-chrome", "--product-version");

        /// <inheritdoc/>
        public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion)
        {
            browserVersion.CheckNotNullOrWhitespace(browserVersion);

            int browserVersionNumbersCount = VersionUtils.GetNumbersCount(browserVersion);

            string browserVersionToUse = browserVersionNumbersCount == 1
                ? browserVersion
                : browserVersionNumbersCount == 2
                    ? VersionUtils.TrimMinor(browserVersion)
                    : VersionUtils.TrimRevision(browserVersion);

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
                .GetString();
        }
    }
}
