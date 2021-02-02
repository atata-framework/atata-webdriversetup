namespace Atata.WebDriverSetup
{
    public class OperaDriverSetupStrategy : GitHubRepositoryBasedDriverSetupStrategy, IDriverSetupStrategy
    {
        public OperaDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
            : base(httpRequestExecutor, "operasoftware", "operachromiumdriver", "v.")
        {
        }

        public string DriverBinaryFileName { get; } =
            OSInfo.IsWindows
                ? "operadriver.exe"
                : "operadriver";

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
