using System;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the Internet Explorer driver (<c>IEDriverServer.exe</c>) setup strategy.
    /// </summary>
    public class InternetExplorerDriverSetupStrategy :
        IDriverSetupStrategy,
        IGetsDriverLatestVersion
    {
        /// <inheritdoc/>
        public string DriverBinaryFileName { get; } = "IEDriverServer.exe";

        /// <inheritdoc/>
        public string GetDriverLatestVersion() => "3.150.1";

        /// <inheritdoc/>
        public Uri GetDriverDownloadUrl(string version)
        {
            string versionTillPatch = VersionUtils.TrimRevision(version);
            string versionTillMinor = VersionUtils.TrimPatch(version);

            string osArchitecture = OSInfo.Is64Bit ? "x64" : "Win32";

            return new Uri($"http://selenium-release.storage.googleapis.com/{versionTillMinor}/IEDriverServer_{osArchitecture}_{versionTillPatch}.zip");
        }
    }
}
