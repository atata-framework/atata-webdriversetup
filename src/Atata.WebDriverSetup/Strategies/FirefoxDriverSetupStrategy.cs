namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the Firefox/Gecko driver (<c>geckodriver.exe</c>/<c>geckodriver</c>) setup strategy.
    /// </summary>
    public class FirefoxDriverSetupStrategy : GitHubRepositoryBasedDriverSetupStrategy, IDriverSetupStrategy
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
        public string DriverBinaryFileName { get; } =
            OSInfo.IsWindows
                ? "geckodriver.exe"
                : "geckodriver";

        protected override string GetDriverDownloadFileName(string version)
        {
            string commonNamePart = $"geckodriver-v{version}-";

            return OSInfo.IsOSX
                ? $"{commonNamePart}macos.tar.gz"
                : OSInfo.IsLinux
                    ? $"{commonNamePart}linux{OSInfo.Bits}.tar.gz"
                    : $"{commonNamePart}win{OSInfo.Bits}.zip";
        }
    }
}
