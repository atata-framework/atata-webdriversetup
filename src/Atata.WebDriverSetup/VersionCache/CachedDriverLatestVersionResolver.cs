using System;

namespace Atata.WebDriverSetup
{
    internal class CachedDriverLatestVersionResolver : IGetsDriverLatestVersion
    {
        private readonly IGetsDriverLatestVersion actualResolver;

        private readonly IDriverVersionCache driverVersionCache;

        private readonly TimeSpan versionCheckInterval;

        internal CachedDriverLatestVersionResolver(
            IGetsDriverLatestVersion actualResolver,
            IDriverVersionCache driverVersionCache,
            TimeSpan versionCheckInterval)
        {
            this.actualResolver = actualResolver;
            this.driverVersionCache = driverVersionCache;
            this.versionCheckInterval = versionCheckInterval;
        }

        public string GetDriverLatestVersion() =>
            driverVersionCache.GetOrAddLatest(
                DateTime.UtcNow - versionCheckInterval,
                actualResolver.GetDriverLatestVersion);
    }
}
