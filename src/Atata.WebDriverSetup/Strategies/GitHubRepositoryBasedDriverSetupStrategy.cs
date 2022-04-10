using System;
using System.Linq;

namespace Atata.WebDriverSetup
{
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

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubRepositoryBasedDriverSetupStrategy"/> class.
        /// </summary>
        /// <param name="httpRequestExecutor">The HTTP request executor.</param>
        /// <param name="organizationName">Name of the GitHub organization.</param>
        /// <param name="repositoryName">Name of the GitHub repository.</param>
        /// <param name="versionTagPrefix">The version tag prefix.</param>
        protected GitHubRepositoryBasedDriverSetupStrategy(
            IHttpRequestExecutor httpRequestExecutor,
            string organizationName,
            string repositoryName,
            string versionTagPrefix = "v")
        {
            _httpRequestExecutor = httpRequestExecutor;
            _versionTagPrefix = versionTagPrefix;
            _baseUrl = $"https://github.com/{organizationName}/{repositoryName}";
        }

        /// <inheritdoc/>
        public abstract string DriverBinaryFileName { get; }

        /// <inheritdoc/>
        public string GetDriverLatestVersion()
        {
            string latestReleaseUrl = $"{_baseUrl}/releases/latest";
            string actualReleaseUrl = _httpRequestExecutor.GetRedirectUrl(latestReleaseUrl).AbsoluteUri;

            return actualReleaseUrl.Split('/').Last().Substring(_versionTagPrefix.Length);
        }

        /// <inheritdoc/>
        public Uri GetDriverDownloadUrl(string version, Architecture architecture) =>
            new Uri($"{_baseUrl}/releases/download/{_versionTagPrefix}{version}/{GetDriverDownloadFileName(version, architecture)}");

        /// <summary>
        /// Gets the name of the driver download file.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="architecture">The architecture.</param>
        /// <returns>The file name.</returns>
        protected abstract string GetDriverDownloadFileName(string version, Architecture architecture);
    }
}
