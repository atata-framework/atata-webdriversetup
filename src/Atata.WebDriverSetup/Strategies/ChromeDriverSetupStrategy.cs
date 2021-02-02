using System;

namespace Atata.WebDriverSetup
{
    public class ChromeDriverSetupStrategy :
        IDriverSetupStrategy,
        IGetsInstalledBrowserVersion,
        IGetsDriverVersionCorrespondingToBrowserVersion
    {
        private const string BaseUrl =
            "https://chromedriver.storage.googleapis.com";

        private const string DriverLatestVersionUrl =
            BaseUrl + "/LATEST_RELEASE";

        private const string DriverSpecificVersionUrlFormat =
            DriverLatestVersionUrl + "_{0}";

        private readonly IHttpRequestExecutor httpRequestExecutor;

        public ChromeDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
        {
            this.httpRequestExecutor = httpRequestExecutor;
        }

        public string DriverBinaryFileName { get; } =
            OSInfo.IsWindows
                ? "chromedriver.exe"
                : "chromedriver";

        public string GetDriverLatestVersion() =>
            httpRequestExecutor.DownloadString(DriverLatestVersionUrl).Trim();

        public Uri GetDriverDownloadUrl(string version) =>
            new Uri($"{BaseUrl}/{version}/{GetDriverDownloadFileName()}");

        private static string GetDriverDownloadFileName() =>
            OSInfo.IsLinux
                ? "chromedriver_linux64.zip"
                : OSInfo.IsOSX
                ? "chromedriver_mac64.zip"
                : "chromedriver_win32.zip";

        public string GetInstalledBrowserVersion() =>
            OSInfo.IsWindows
                ? AppVersionDetector.GetFromProgramFiles(@"Google\Chrome\Application\chrome.exe")
                    ?? AppVersionDetector.GetFromBLBeaconInRegistry(@"Google\Chrome")
                    ?? AppVersionDetector.GetByApplicationPathFromRegistry("chrome.exe")
                : null;

        public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion)
        {
            int browserVersionNumbersCount = VersionUtils.GetNumbersCount(browserVersion);

            string browserVersionToUse = browserVersionNumbersCount == 1
                ? browserVersion
                : browserVersionNumbersCount == 2
                    ? VersionUtils.TrimMinor(browserVersion)
                    : VersionUtils.TrimRevision(browserVersion);

            string url = string.Format(DriverSpecificVersionUrlFormat, browserVersionToUse);

            return httpRequestExecutor.DownloadString(url);
        }
    }
}
