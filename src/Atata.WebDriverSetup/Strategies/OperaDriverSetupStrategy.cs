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
        protected override string GetDriverDownloadFileName(string version)
        {
            string commonNamePart = $"operadriver_";

            return OSInfo.IsOSX
                ? $"{commonNamePart}mac64.zip"
                : OSInfo.IsLinux
                    ? $"{commonNamePart}linux64.zip"
                    : $"{commonNamePart}win{OSInfo.Bits}.zip";
        }
    }
}
