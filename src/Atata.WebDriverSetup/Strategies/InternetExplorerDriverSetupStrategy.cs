using System;

namespace Atata.WebDriverSetup
{
    public class InternetExplorerDriverSetupStrategy : IDriverSetupStrategy
    {
        public string DriverBinaryFileName { get; } = "IEDriverServer.exe";

        public string GetDriverLatestVersion() => "3.150.1";

        public Uri GetDriverDownloadUrl(string version)
        {
            string versionTillPatch = VersionUtils.TrimRevision(version);
            string versionTillMinor = VersionUtils.TrimPatch(version);

            string osArchitecture = OSInfo.Is64Bit ? "x64" : "Win32";

            return new Uri($"http://selenium-release.storage.googleapis.com/{versionTillMinor}/IEDriverServer_{osArchitecture}_{versionTillPatch}.zip");
        }
    }
}
