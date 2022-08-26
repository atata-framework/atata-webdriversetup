using System.Text.RegularExpressions;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the Firefox/Gecko driver (<c>geckodriver.exe</c>/<c>geckodriver</c>) setup strategy.
    /// </summary>
    public class FirefoxDriverSetupStrategy :
        GitHubRepositoryBasedDriverSetupStrategy,
        IGetsInstalledBrowserVersion
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
        public string GetInstalledBrowserVersion() =>
            OSInfo.IsWindows
                ? AppVersionDetector.GetFromProgramFiles(@"Mozilla Firefox\firefox.exe")
                    ?? AppVersionDetector.GetFromApplicationKeyInRegistry(@"Mozilla\Mozilla Firefox")
                    ?? AppVersionDetector.GetByApplicationPathInRegistry("firefox.exe")
                : OSInfo.IsOSX
                    ? AppVersionDetector.GetThroughOSXApplicationCli("Mozilla Firefox")
                    : AppVersionDetector.GetThroughCli(
                        "firefox",
                        "--version",
                        (output) => new Regex(@"(?<=^|\s)\d\S+").Match(output).Value);

        /// <inheritdoc/>
        protected override string GetDriverDownloadFileName(string version, Architecture architecture)
        {
            string commonNamePart = $"geckodriver-v{version}-";

            return OSInfo.IsWindows
                ? $"{commonNamePart}win{architecture.GetBits()}.zip"
                : OSInfo.IsOSX
                    ? $"{commonNamePart}macos.tar.gz"
                    : $"{commonNamePart}linux{architecture.GetBits()}.tar.gz";
        }
    }
}
