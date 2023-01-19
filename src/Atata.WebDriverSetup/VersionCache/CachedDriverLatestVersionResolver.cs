using System;

namespace Atata.WebDriverSetup
{
    internal sealed class CachedDriverLatestVersionResolver : IGetsDriverLatestVersion
    {
        private readonly IGetsDriverLatestVersion _actualResolver;

        private readonly IDriverVersionCache _driverVersionCache;

        private readonly TimeSpan _versionCheckInterval;

        internal CachedDriverLatestVersionResolver(
            IGetsDriverLatestVersion actualResolver,
            IDriverVersionCache driverVersionCache,
            TimeSpan versionCheckInterval)
        {
            _actualResolver = actualResolver;
            _driverVersionCache = driverVersionCache;
            _versionCheckInterval = versionCheckInterval;
        }

        public string GetDriverLatestVersion() =>
            _driverVersionCache.GetOrAddLatest(
                DateTime.UtcNow - _versionCheckInterval,
                _actualResolver.GetDriverLatestVersion);
    }
}
