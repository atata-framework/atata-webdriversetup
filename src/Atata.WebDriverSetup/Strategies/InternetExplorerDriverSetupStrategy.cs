using System.Collections.Generic;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the Internet Explorer driver (<c>IEDriverServer.exe</c>) setup strategy.
    /// </summary>
    public class InternetExplorerDriverSetupStrategy : GitHubRepositoryBasedDriverSetupStrategy
    {
        private static readonly Dictionary<string, string> s_driverVersionToReleaseVersionMappings = new Dictionary<string, string>
        {
            ["4.8.1"] = "4.8.0"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriverSetupStrategy"/> class.
        /// </summary>
        /// <param name="httpRequestExecutor">The HTTP request executor.</param>
        public InternetExplorerDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
            : base(httpRequestExecutor, "SeleniumHQ", "selenium", "selenium-", s_driverVersionToReleaseVersionMappings)
        {
        }

        /// <inheritdoc/>
        public override string DriverBinaryFileName { get; } = "IEDriverServer.exe";

        /// <inheritdoc/>
        public override string GetDriverLatestVersion()
        {
            string version = base.GetDriverLatestVersion();

            return version == "4.9.0"
                ? "4.8.1"
                : version;
        }

        protected override string GetDriverDownloadFileName(string version, Architecture architecture) =>
            $"IEDriverServer_{(architecture == Architecture.X32 ? "Win32" : "x64")}_{version}.zip";
    }
}
