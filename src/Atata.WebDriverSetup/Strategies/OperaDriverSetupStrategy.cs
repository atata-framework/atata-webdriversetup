namespace Atata.WebDriverSetup;

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
    public override string GetDriverBinaryFileName(TargetOSPlatform platform) =>
        platform.OSFamily == TargetOSFamily.Windows
            ? "operadriver.exe"
            : "operadriver";

    /// <inheritdoc/>
    protected override string GetDriverDownloadFileName(string version, TargetOSPlatform platform)
    {
        const string commonNamePart = "operadriver_";

        return platform.OSFamily switch
        {
            TargetOSFamily.Windows => $"{commonNamePart}win{platform.Bits}.zip",
            TargetOSFamily.Mac => $"{commonNamePart}mac64.zip",
            _ => $"{commonNamePart}linux64.zip"
        };
    }
}
