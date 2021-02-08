using System;

namespace Atata.WebDriverSetup
{
    internal class CachedDriverVersionCorrespondingToBrowserVersionResolver
        : IGetsDriverVersionCorrespondingToBrowserVersion
    {
        private readonly IGetsDriverVersionCorrespondingToBrowserVersion actualResolver;

        private readonly IDriverVersionCache driverVersionCache;

        private readonly TimeSpan versionCheckInterval;

        internal CachedDriverVersionCorrespondingToBrowserVersionResolver(
            IGetsDriverVersionCorrespondingToBrowserVersion actualResolver,
            IDriverVersionCache driverVersionCache,
            TimeSpan versionCheckInterval)
        {
            this.actualResolver = actualResolver;
            this.driverVersionCache = driverVersionCache;
            this.versionCheckInterval = versionCheckInterval;
        }

        public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion) =>
            driverVersionCache.GetOrAdd(
                browserVersion,
                DateTime.UtcNow - versionCheckInterval,
                actualResolver.GetDriverVersionCorrespondingToBrowserVersion);
    }
}
