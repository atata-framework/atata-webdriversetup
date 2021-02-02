using System;

namespace Atata.WebDriverSetup
{
    public class EdgeDriverSetupStrategy :
        IDriverSetupStrategy,
        IGetsInstalledBrowserVersion,
        IGetsDriverVersionCorrespondingToBrowserVersion
    {
        private const string BaseUrl =
            "https://msedgedriver.azureedge.net";

        private const string DriverLatestVersionUrl =
            BaseUrl + "/LATEST_STABLE";

        private readonly IHttpRequestExecutor httpRequestExecutor;

        public EdgeDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
        {
            this.httpRequestExecutor = httpRequestExecutor;
        }

        public string DriverBinaryFileName { get; } =
            OSInfo.IsWindows
                ? "msedgedriver.exe"
                : "msedgedriver";

        public string GetDriverLatestVersion() =>
            httpRequestExecutor.DownloadString(DriverLatestVersionUrl).Trim();

        public Uri GetDriverDownloadUrl(string version) =>
            new Uri($"{BaseUrl}/{version}/{GetDriverDownloadFileName()}");

        private static string GetDriverDownloadFileName()
        {
            return OSInfo.IsOSX
                ? "edgedriver_mac64.zip"
                : OSInfo.IsWindows
                    ? $"edgedriver_win{OSInfo.Bits}.zip"
                    : throw new DriverSetupException("Linux OS is currently unsupported for a driver setup.");
        }

        public string GetInstalledBrowserVersion() =>
            OSInfo.IsWindows
                ? AppVersionDetector.GetFromProgramFiles(@"Microsoft\Edge\Application\msedge.exe")
                    ?? AppVersionDetector.GetFromBLBeaconInRegistry(@"Microsoft\Edge")
                    ?? AppVersionDetector.GetByApplicationPathFromRegistry("msedge.exe")
                : null;

        public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion) =>
            browserVersion;
    }
}
