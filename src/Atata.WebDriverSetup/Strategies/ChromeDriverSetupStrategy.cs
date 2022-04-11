using System;

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
        private const string BaseUrl =
            "https://chromedriver.storage.googleapis.com";

        private const string DriverLatestVersionUrl =
            BaseUrl + "/LATEST_RELEASE";

        private const string DriverSpecificVersionUrlFormat =
            DriverLatestVersionUrl + "_{0}";

        private readonly IHttpRequestExecutor _httpRequestExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeDriverSetupStrategy"/> class.
        /// </summary>
        /// <param name="httpRequestExecutor">The HTTP request executor.</param>
        public ChromeDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
        {
            _httpRequestExecutor = httpRequestExecutor;
        }

        /// <inheritdoc/>
        public string DriverBinaryFileName { get; } =
            OSInfo.IsWindows
                ? "chromedriver.exe"
                : "chromedriver";

        /// <inheritdoc/>
        public string GetDriverLatestVersion() =>
            _httpRequestExecutor.DownloadString(DriverLatestVersionUrl).Trim();

        /// <inheritdoc/>
        public Uri GetDriverDownloadUrl(string version, Architecture architecture) =>
            new Uri($"{BaseUrl}/{version}/{GetDriverDownloadFileName()}");

        private static string GetDriverDownloadFileName() =>
            OSInfo.IsWindows
                ? "chromedriver_win32.zip"
                : OSInfo.IsOSX
                    ? "chromedriver_mac64.zip"
                    : "chromedriver_linux64.zip";

        /// <inheritdoc/>
        public string GetInstalledBrowserVersion() =>
            OSInfo.IsWindows
                ? AppVersionDetector.GetFromProgramFiles(@"Google\Chrome\Application\chrome.exe")
                    ?? AppVersionDetector.GetFromBLBeaconInRegistry(@"Google\Chrome")
                    ?? AppVersionDetector.GetByApplicationPathInRegistry("chrome.exe")
                : OSInfo.IsOSX
                    ? AppVersionDetector.GetThroughOSXApplicationCli("Google Chrome")
                    : AppVersionDetector.GetThroughCli("google-chrome", "--product-version");

        /// <inheritdoc/>
        public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion)
        {
            int browserVersionNumbersCount = VersionUtils.GetNumbersCount(browserVersion);

            string browserVersionToUse = browserVersionNumbersCount == 1
                ? browserVersion
                : browserVersionNumbersCount == 2
                    ? VersionUtils.TrimMinor(browserVersion)
                    : VersionUtils.TrimRevision(browserVersion);

            string url = string.Format(DriverSpecificVersionUrlFormat, browserVersionToUse);

            return _httpRequestExecutor.DownloadString(url);
        }
    }
}
