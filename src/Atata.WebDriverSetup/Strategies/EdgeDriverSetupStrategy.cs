using System;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the Edge driver (<c>msedgedriver.exe</c>/<c>msedgedriver</c>) setup strategy.
    /// </summary>
    public class EdgeDriverSetupStrategy :
        IDriverSetupStrategy,
        IGetsDriverLatestVersion,
        IGetsInstalledBrowserVersion,
        IGetsDriverVersionCorrespondingToBrowserVersion
    {
        private const string BaseUrl =
            "https://msedgedriver.azureedge.net";

        private const string DriverLatestVersionUrl =
            BaseUrl + "/LATEST_STABLE";

        private readonly IHttpRequestExecutor _httpRequestExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriverSetupStrategy"/> class.
        /// </summary>
        /// <param name="httpRequestExecutor">The HTTP request executor.</param>
        public EdgeDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
        {
            _httpRequestExecutor = httpRequestExecutor;
        }

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
            new Uri($"{BaseUrl}/{version}/{GetDriverDownloadFileName(architecture)}");

        private static string GetDriverDownloadFileName(Architecture architecture) =>
            OSInfo.IsWindows
                ? $"edgedriver_win{architecture.GetBits()}.zip"
                : OSInfo.IsOSX
                    ? "edgedriver_mac64.zip"
                    : "edgedriver_linux64.zip";

        /// <inheritdoc/>
        public string GetInstalledBrowserVersion() =>
            OSInfo.IsWindows
                ? AppVersionDetector.GetFromProgramFiles(@"Microsoft\Edge\Application\msedge.exe")
                    ?? AppVersionDetector.GetFromBLBeaconInRegistry(@"Microsoft\Edge")
                    ?? AppVersionDetector.GetByApplicationPathInRegistry("msedge.exe")
                : null;

        /// <inheritdoc/>
        public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion) =>
            browserVersion;
    }
}
