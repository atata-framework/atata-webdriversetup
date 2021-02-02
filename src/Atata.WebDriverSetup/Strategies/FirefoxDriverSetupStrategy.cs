namespace Atata.WebDriverSetup
{
    public class FirefoxDriverSetupStrategy : GitHubRepositoryBasedDriverSetupStrategy, IDriverSetupStrategy
    {
        public FirefoxDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
            : base(httpRequestExecutor, "mozilla", "geckodriver")
        {
        }

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
