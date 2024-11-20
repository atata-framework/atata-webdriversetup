namespace Atata.WebDriverSetup.OS;

internal readonly struct VersionOSPlatformsPair
{
    public readonly string Version;

    public readonly OSPlatforms OSPlatforms;

    public VersionOSPlatformsPair(string version, OSPlatforms osPlatforms)
    {
        Version = version;
        OSPlatforms = osPlatforms;
    }
}
