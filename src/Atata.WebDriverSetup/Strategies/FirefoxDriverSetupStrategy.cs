using System;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the Firefox/Gecko driver (<c>geckodriver.exe</c>/<c>geckodriver</c>) setup strategy.
    /// </summary>
    public class FirefoxDriverSetupStrategy :
        GitHubRepositoryBasedDriverSetupStrategy,
        IGetsInstalledBrowserVersion,
        IGetsDriverVersionCorrespondingToBrowserVersion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriverSetupStrategy"/> class.
        /// </summary>
        /// <param name="httpRequestExecutor">The HTTP request executor.</param>
        public FirefoxDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
            : base(httpRequestExecutor, "mozilla", "geckodriver")
        {
        }

        /// <inheritdoc/>
        public override string DriverBinaryFileName { get; } =
            OSInfo.IsWindows
                ? "geckodriver.exe"
                : "geckodriver";

        /// <inheritdoc/>
        protected override string GetDriverDownloadFileName(string version, Architecture architecture)
        {
            string commonNamePart = $"geckodriver-v{version}-";

            return OSInfo.IsWindows
                ? $"{commonNamePart}win{GetArchitectureSuffix(architecture)}.zip"
                : OSInfo.IsOSX
                    ? $"{commonNamePart}macos{(architecture == Architecture.Arm64 ? GetArchitectureSuffix(Architecture.Arm64) : null)}.tar.gz"
                    : $"{commonNamePart}linux{GetArchitectureSuffix(architecture)}.tar.gz";
        }

        private static string GetArchitectureSuffix(Architecture architecture)
        {
            switch (architecture)
            {
                case Architecture.X32:
                    return "32";
                case Architecture.X64:
                    return "64";
                case Architecture.Arm64:
                    return "-aarch64";
                default:
                    throw new ArgumentException($@"Unsupported ""{architecture}"" architecture.", nameof(architecture));
            }
        }

        /// <inheritdoc/>
        public string GetInstalledBrowserVersion() =>
            OSInfo.IsWindows
                ? AppVersionDetector.GetFromProgramFiles(@"Mozilla Firefox\firefox.exe")
                    ?? RegistryUtils.GetValue(@"HKEY_CURRENT_USER\Software\Mozilla\Mozilla Firefox")
                    ?? AppVersionDetector.GetByApplicationPathInRegistry("firefox.exe")
                : (OSInfo.IsOSX
                    ? AppVersionDetector.GetThroughOSXApplicationCli("Firefox")
                    : AppVersionDetector.GetThroughCli("firefox", "-v"))
                    ?.Replace("Mozilla Firefox ", null);

        /// <inheritdoc/>
        public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion)
        {
            browserVersion.CheckNotNullOrWhitespace(browserVersion);

            string browserMajorVersion = VersionUtils.TrimMinor(browserVersion);
            int browserMajorVersionNumber = int.Parse(browserMajorVersion);

            return browserMajorVersionNumber >= 91
                ? "0.31.0"
                : browserMajorVersionNumber >= 78
                    ? "0.30.0"
                    : "0.29.1";
        }
    }
}
