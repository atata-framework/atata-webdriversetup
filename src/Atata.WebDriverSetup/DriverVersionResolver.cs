using System.IO;

namespace Atata.WebDriverSetup
{
    internal class DriverVersionResolver
    {
        private readonly string browserName;

        private readonly DriverSetupOptions options;

        private readonly IDriverSetupStrategy setupStrategy;

        internal DriverVersionResolver(
            string browserName,
            DriverSetupOptions options,
            IDriverSetupStrategy setupStrategy)
        {
            this.browserName = browserName;
            this.options = options;
            this.setupStrategy = setupStrategy;
        }

        internal string ResolveByBrowserVersion(string version) =>
            GetCorrespondingVersionResolver().GetDriverVersionCorrespondingToBrowserVersion(version)
                ?? throw new DriverSetupException(
                    $"Failed to find {browserName} driver version corresponding to browser {version} version.");

        internal string ResolveLatestVersion() =>
            GetLatestVersionResolver().GetDriverLatestVersion()
                ?? throw new DriverSetupException(
                    $"Failed to find {browserName} driver latest version.");

        internal string ResolveCorrespondingOrLatestVersion() =>
            ResolveCorrespondingVersion()
                ?? ResolveLatestVersion();

        private string ResolveCorrespondingVersion()
        {
            string installedVersion = (setupStrategy as IGetsInstalledBrowserVersion)
                ?.GetInstalledBrowserVersion();

            return installedVersion != null
                ? ResolveByBrowserVersion(installedVersion)
                : null;
        }

        private IGetsDriverLatestVersion GetLatestVersionResolver()
        {
            if (setupStrategy is IGetsDriverLatestVersion resolver)
            {
                return options.UseVersionCache
                    ? new CachedDriverLatestVersionResolver(
                        resolver,
                        GetDriverVersionCache(),
                        options.LatestVersionCheckInterval)
                    : resolver;
            }
            else
            {
                throw new DriverSetupException(
                    $"Cannot get {browserName} driver latest version as " +
                    $"{setupStrategy.GetType().FullName} doesn't support that feature. " +
                    $"It should implement {typeof(IGetsDriverLatestVersion).FullName}.");
            }
        }

        private IGetsDriverVersionCorrespondingToBrowserVersion GetCorrespondingVersionResolver()
        {
            if (setupStrategy is IGetsDriverVersionCorrespondingToBrowserVersion resolver)
            {
                return options.UseVersionCache
                    ? new CachedDriverVersionCorrespondingToBrowserVersionResolver(
                        resolver,
                        GetDriverVersionCache(),
                        options.SpecificVersionCheckInterval)
                    : resolver;
            }
            else
            {
                throw new DriverSetupException(
                    $"Cannot get {browserName} driver version corresponding to browser version as " +
                    $"{setupStrategy.GetType().Name} doesn't support that feature. " +
                    $"It should implement {typeof(IGetsDriverVersionCorrespondingToBrowserVersion).FullName}.");
            }
        }

        private IDriverVersionCache GetDriverVersionCache()
        {
            return new XmlFileDriverVersionCache(
                Path.Combine(
                    options.StorageDirectoryPath,
                    browserName.Replace(" ", null).ToLower(),
                    "versioncache.xml"));
        }
    }
}
