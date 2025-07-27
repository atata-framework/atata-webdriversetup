namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the base class for the driver setup strategies that are based on GitHub repository as a driver storage.
/// </summary>
public abstract class GitHubRepositoryBasedDriverSetupStrategy :
    IDriverSetupStrategy,
    IGetsDriverLatestVersion
{
    private readonly IHttpRequestExecutor _httpRequestExecutor;

    private readonly string _versionTagPrefix;

    private readonly string _baseUrl;

    private readonly Dictionary<string, string>? _driverVersionToReleaseVersionMappings;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubRepositoryBasedDriverSetupStrategy"/> class.
    /// </summary>
    /// <param name="httpRequestExecutor">The HTTP request executor.</param>
    /// <param name="organizationName">Name of the GitHub organization.</param>
    /// <param name="repositoryName">Name of the GitHub repository.</param>
    /// <param name="versionTagPrefix">The version tag prefix.</param>
    /// <param name="driverVersionToReleaseVersionMappings">The mappings of driver version to GitHub release version.</param>
    protected GitHubRepositoryBasedDriverSetupStrategy(
        IHttpRequestExecutor httpRequestExecutor,
        string organizationName,
        string repositoryName,
        string versionTagPrefix = "v",
        Dictionary<string, string>? driverVersionToReleaseVersionMappings = null)
    {
        _httpRequestExecutor = httpRequestExecutor;
        _versionTagPrefix = versionTagPrefix;
        _baseUrl = $"https://github.com/{organizationName}/{repositoryName}";
        _driverVersionToReleaseVersionMappings = driverVersionToReleaseVersionMappings;
    }

    /// <inheritdoc/>
    public abstract string GetDriverBinaryFileName(TargetOSPlatform platform);

    /// <inheritdoc/>
    public virtual async Task<string> GetDriverLatestVersionAsync(CancellationToken cancellationToken = default)
    {
        string latestReleaseUrl = $"{_baseUrl}/releases/latest";

        Uri redirectUri = await _httpRequestExecutor.GetRedirectUrlAsync(latestReleaseUrl, cancellationToken)
            .ConfigureAwait(false);
        string actualReleaseUrl = redirectUri.AbsoluteUri;

        string[] urlParts = actualReleaseUrl.Split('/');
        return urlParts[^1][_versionTagPrefix.Length..];
    }

    /// <inheritdoc/>
    public Uri GetDriverDownloadUrl(string version, TargetOSPlatform platform)
    {
        if (_driverVersionToReleaseVersionMappings is null
            || !_driverVersionToReleaseVersionMappings.TryGetValue(version, out string releaseVersion))
            releaseVersion = version;

        return BuildDriverDownloadUrl(releaseVersion, version, platform);
    }

    /// <summary>
    /// Builds the driver download URL.
    /// </summary>
    /// <param name="releaseVersion">The release version.</param>
    /// <param name="driverVersion">The driver version.</param>
    /// <param name="platform">The target OS platform.</param>
    /// <returns>The driver download URL.</returns>
    protected virtual Uri BuildDriverDownloadUrl(string releaseVersion, string driverVersion, TargetOSPlatform platform) =>
        new($"{_baseUrl}/releases/download/{_versionTagPrefix}{releaseVersion}/{GetDriverDownloadFileName(driverVersion, platform)}");

    /// <summary>
    /// Gets the name of the driver download file.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="platform">The target OS platform.</param>
    /// <returns>The file name.</returns>
    protected abstract string GetDriverDownloadFileName(string version, TargetOSPlatform platform);
}
