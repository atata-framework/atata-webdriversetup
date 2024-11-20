namespace Atata.WebDriverSetup;

internal sealed class CachedDriverVersionCorrespondingToBrowserVersionResolver
    : IGetsDriverVersionCorrespondingToBrowserVersion
{
    private readonly IGetsDriverVersionCorrespondingToBrowserVersion _actualResolver;

    private readonly IDriverVersionCache _driverVersionCache;

    private readonly TimeSpan _versionCheckInterval;

    internal CachedDriverVersionCorrespondingToBrowserVersionResolver(
        IGetsDriverVersionCorrespondingToBrowserVersion actualResolver,
        IDriverVersionCache driverVersionCache,
        TimeSpan versionCheckInterval)
    {
        _actualResolver = actualResolver;
        _driverVersionCache = driverVersionCache;
        _versionCheckInterval = versionCheckInterval;
    }

    public string GetDriverVersionCorrespondingToBrowserVersion(string browserVersion, TargetOSPlatform platform) =>
        _driverVersionCache.GetOrAdd(
            browserVersion,
            DateTime.UtcNow - _versionCheckInterval,
            v => _actualResolver.GetDriverVersionCorrespondingToBrowserVersion(v, platform));
}
