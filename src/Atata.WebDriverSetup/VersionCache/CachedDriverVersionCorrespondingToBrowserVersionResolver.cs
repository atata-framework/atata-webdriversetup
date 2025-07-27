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

    public async Task<string> GetDriverVersionCorrespondingToBrowserVersionAsync(
        string browserVersion,
        TargetOSPlatform platform,
        CancellationToken cancellationToken = default)
        =>
        await _driverVersionCache.GetOrAddAsync(
            browserVersion,
            DateTime.UtcNow - _versionCheckInterval,
            (v, ct) => _actualResolver.GetDriverVersionCorrespondingToBrowserVersionAsync(v, platform, ct),
            cancellationToken)
            .ConfigureAwait(false);
}
