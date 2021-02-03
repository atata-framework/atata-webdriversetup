using System;
using System.Linq;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the base class for the driver setup strategies that are based on GitHub repository as a driver storage.
    /// </summary>
    public abstract class GitHubRepositoryBasedDriverSetupStrategy
    {
        private readonly IHttpRequestExecutor httpRequestExecutor;

        private readonly string versionTagPrefix;

        private readonly string baseUrl;

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
            this.httpRequestExecutor = httpRequestExecutor;
            this.versionTagPrefix = versionTagPrefix;
            baseUrl = $"https://github.com/{organizationName}/{repositoryName}";
        }

        public string GetDriverLatestVersion()
        {
            string latestReleaseUrl = $"{baseUrl}/releases/latest";
            string actualReleaseUrl = httpRequestExecutor.GetRedirectUrl(latestReleaseUrl).AbsoluteUri;

            return actualReleaseUrl.Split('/').Last().Substring(versionTagPrefix.Length);
        }

        public Uri GetDriverDownloadUrl(string version) =>
            new Uri($"{baseUrl}/releases/download/{versionTagPrefix}{version}/{GetDriverDownloadFileName(version)}");

        protected abstract string GetDriverDownloadFileName(string version);
    }
}
