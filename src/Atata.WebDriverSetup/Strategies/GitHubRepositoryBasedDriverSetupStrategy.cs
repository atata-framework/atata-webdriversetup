using System;
using System.Linq;

namespace Atata.WebDriverSetup
{
    public abstract class GitHubRepositoryBasedDriverSetupStrategy
    {
        private readonly IHttpRequestExecutor httpRequestExecutor;

        private readonly string versionTagPrefix;

        private readonly string baseUrl;

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
