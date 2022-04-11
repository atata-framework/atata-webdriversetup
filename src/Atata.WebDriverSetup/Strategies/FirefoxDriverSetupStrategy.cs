namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the Firefox/Gecko driver (<c>geckodriver.exe</c>/<c>geckodriver</c>) setup strategy.
    /// </summary>
    public class FirefoxDriverSetupStrategy :
        GitHubRepositoryBasedDriverSetupStrategy
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
                ? $"{commonNamePart}win{architecture.GetBits()}.zip"
                : OSInfo.IsOSX
                    ? $"{commonNamePart}macos.tar.gz"
                    : $"{commonNamePart}linux{architecture.GetBits()}.tar.gz";
        }
    }
}
