namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the Internet Explorer driver (<c>IEDriverServer.exe</c>) setup strategy.
    /// </summary>
    public class InternetExplorerDriverSetupStrategy : GitHubRepositoryBasedDriverSetupStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternetExplorerDriverSetupStrategy"/> class.
        /// </summary>
        /// <param name="httpRequestExecutor">The HTTP request executor.</param>
        public InternetExplorerDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
            : base(httpRequestExecutor, "SeleniumHQ", "selenium", "selenium-")
        {
        }

        /// <inheritdoc/>
        public override string DriverBinaryFileName { get; } = "IEDriverServer.exe";

        protected override string GetDriverDownloadFileName(string version, Architecture architecture) =>
            $"IEDriverServer_{(architecture == Architecture.X32 ? "Win32" : "x64")}_{version}.zip";
    }
}
