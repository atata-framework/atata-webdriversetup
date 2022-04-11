namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the Opera driver (<c>operadriver.exe</c>/<c>operadriver</c>) setup strategy.
    /// </summary>
    public class OperaDriverSetupStrategy :
        GitHubRepositoryBasedDriverSetupStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperaDriverSetupStrategy"/> class.
        /// </summary>
        /// <param name="httpRequestExecutor">The HTTP request executor.</param>
        public OperaDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
            : base(httpRequestExecutor, "operasoftware", "operachromiumdriver", "v.")
        {
        }

        /// <inheritdoc/>
        public override string DriverBinaryFileName { get; } =
            OSInfo.IsWindows
                ? "operadriver.exe"
                : "operadriver";

        /// <inheritdoc/>
        protected override string GetDriverDownloadFileName(string version, Architecture architecture)
        {
            const string commonNamePart = "operadriver_";

            return OSInfo.IsWindows
                ? $"{commonNamePart}win{architecture.GetBits()}.zip"
                : OSInfo.IsOSX
                    ? $"{commonNamePart}mac64.zip"
                    : $"{commonNamePart}linux64.zip";
        }
    }
}
