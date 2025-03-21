﻿namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the Internet Explorer driver (<c>IEDriverServer.exe</c>) setup strategy.
/// </summary>
public class InternetExplorerDriverSetupStrategy : GitHubRepositoryBasedDriverSetupStrategy
{
    private static readonly Dictionary<string, string> s_driverVersionToReleaseVersionMappings = new()
    {
        ["4.8.1"] = "4.8.0"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="InternetExplorerDriverSetupStrategy"/> class.
    /// </summary>
    /// <param name="httpRequestExecutor">The HTTP request executor.</param>
    public InternetExplorerDriverSetupStrategy(IHttpRequestExecutor httpRequestExecutor)
        : base(httpRequestExecutor, "SeleniumHQ", "selenium", "selenium-", s_driverVersionToReleaseVersionMappings)
    {
    }

    /// <inheritdoc/>
    public override string GetDriverBinaryFileName(TargetOSPlatform platform) =>
        "IEDriverServer.exe";

    /// <inheritdoc/>
    public override string GetDriverLatestVersion() =>
        "4.14.0"; // Temporary workaround, as IEDriver and Selenium releases are not synchronized.

    protected override string GetDriverDownloadFileName(string version, TargetOSPlatform platform) =>
        $"IEDriverServer_{(platform.Architecture == TargetArchitecture.X32 ? "Win32" : "x64")}_{version}.zip";
}
