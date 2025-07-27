namespace Atata.WebDriverSetup;

internal sealed class DriverVersionResolver
{
    private readonly string _browserName;

    private readonly DriverSetupOptions _options;

    private readonly IDriverSetupStrategy _setupStrategy;

    private readonly TargetOSPlatform _platform;

    internal DriverVersionResolver(
        string browserName,
        DriverSetupOptions options,
        IDriverSetupStrategy setupStrategy)
    {
        _browserName = browserName;
        _options = options;
        _setupStrategy = setupStrategy;

        _platform = TargetOSPlatform.Detect(_options.Architecture);
    }

    internal async Task<string> ResolveByBrowserVersionAsync(string version, CancellationToken cancellationToken) =>
        (await GetCorrespondingVersionResolver().GetDriverVersionCorrespondingToBrowserVersionAsync(version, _platform, cancellationToken).ConfigureAwait(false))
            ?? throw new DriverSetupException(
                $"Failed to find {_browserName} driver version corresponding to browser {version} version.");

    internal async Task<string> ResolveLatestVersionAsync(CancellationToken cancellationToken) =>
        (await GetLatestVersionResolver().GetDriverLatestVersionAsync(cancellationToken).ConfigureAwait(false))
            ?? throw new DriverSetupException(
                $"Failed to find {_browserName} driver latest version.");

    internal async Task<string> ResolveCorrespondingOrLatestVersionAsync(CancellationToken cancellationToken) =>
        (await ResolveCorrespondingVersionAsync(cancellationToken).ConfigureAwait(false))
            ?? (await ResolveLatestVersionAsync(cancellationToken).ConfigureAwait(false));

    internal async Task<string?> ResolveClosestVersionAsync(string version, CancellationToken cancellationToken)
    {
        if (_setupStrategy is IGetsDriverClosestVersion closestVersionResolver)
        {
            try
            {
                return await closestVersionResolver.GetDriverClosestVersionAsync(version, _platform, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Log.Warn(exception, $"Failed to resolve driver version closest to {version}.");
            }
        }

        return null;
    }

    private async Task<string?> ResolveCorrespondingVersionAsync(CancellationToken cancellationToken)
    {
        if (_setupStrategy is IGetsInstalledBrowserVersion installedBrowserVersionResolver)
        {
            string? installedVersion = await installedBrowserVersionResolver.GetInstalledBrowserVersionAsync(cancellationToken)
                .ConfigureAwait(false);

            if (installedVersion is not null)
                return await ResolveByBrowserVersionAsync(installedVersion, cancellationToken)
                    .ConfigureAwait(false);
        }

        return null;
    }

    private IGetsDriverLatestVersion GetLatestVersionResolver()
    {
        if (_setupStrategy is IGetsDriverLatestVersion resolver)
        {
            return _options.UseVersionCache
                ? new CachedDriverLatestVersionResolver(
                    resolver,
                    GetDriverVersionCache(),
                    _options.LatestVersionCheckInterval)
                : resolver;
        }
        else
        {
            throw new DriverSetupException(
                $"Cannot get {_browserName} driver latest version as " +
                $"{_setupStrategy.GetType().FullName} doesn't support that feature. " +
                $"It should implement {typeof(IGetsDriverLatestVersion).FullName}.");
        }
    }

    private IGetsDriverVersionCorrespondingToBrowserVersion GetCorrespondingVersionResolver()
    {
        if (_setupStrategy is IGetsDriverVersionCorrespondingToBrowserVersion resolver)
        {
            return _options.UseVersionCache
                ? new CachedDriverVersionCorrespondingToBrowserVersionResolver(
                    resolver,
                    GetDriverVersionCache(),
                    _options.SpecificVersionCheckInterval)
                : resolver;
        }
        else
        {
            throw new DriverSetupException(
                $"Cannot get {_browserName} driver version corresponding to browser version as " +
                $"{_setupStrategy.GetType().Name} doesn't support that feature. " +
                $"It should implement {typeof(IGetsDriverVersionCorrespondingToBrowserVersion).FullName}.");
        }
    }

    private XmlFileDriverVersionCache GetDriverVersionCache() =>
        new(
            Path.Combine(
                _options.StorageDirectoryPath,
                _browserName.Replace(" ", null).ToLower(CultureInfo.InvariantCulture),
                "versioncache.xml"));
}
