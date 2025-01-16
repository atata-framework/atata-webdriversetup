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

    internal string ResolveByBrowserVersion(string version) =>
        GetCorrespondingVersionResolver().GetDriverVersionCorrespondingToBrowserVersion(version, _platform)
            ?? throw new DriverSetupException(
                $"Failed to find {_browserName} driver version corresponding to browser {version} version.");

    internal string ResolveLatestVersion() =>
        GetLatestVersionResolver().GetDriverLatestVersion()
            ?? throw new DriverSetupException(
                $"Failed to find {_browserName} driver latest version.");

    internal string ResolveCorrespondingOrLatestVersion() =>
        ResolveCorrespondingVersion()
            ?? ResolveLatestVersion();

    internal bool TryResolveClosestVersion(string version, out string closestVersion)
    {
        if (_setupStrategy is IGetsDriverClosestVersion closestVersionResolver)
        {
            try
            {
                if (closestVersionResolver.TryGetDriverClosestVersion(version, _platform, out closestVersion))
                    return true;
            }
            catch (Exception exception)
            {
                Log.Warn(exception, $"Failed to resolve driver version closest to {version}.");
            }
        }

        closestVersion = null;
        return false;
    }

    private string ResolveCorrespondingVersion()
    {
        string installedVersion = (_setupStrategy as IGetsInstalledBrowserVersion)
            ?.GetInstalledBrowserVersion();

        return installedVersion != null
            ? ResolveByBrowserVersion(installedVersion)
            : null;
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
