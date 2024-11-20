namespace Atata.WebDriverSetup;

internal static partial class EdgeDriverVersionsMap
{
    internal static bool TryGetDriverVersionCorrespondingToBrowserVersion(string browserVersion, OSPlatforms platform, out string driverVersion)
    {
        int browserVersionMapIndex = FindMapIndexByVersion(browserVersion);

        if (browserVersionMapIndex != -1)
        {
            driverVersion = FindClosestVersionByPlatform(browserVersionMapIndex, platform);
            return driverVersion is not null;
        }

        driverVersion = null;
        return false;
    }

    internal static bool TryGetDriverVersionClosestToBrowserVersion(string browserVersion, OSPlatforms platform, out string driverVersion)
    {
        int browserVersionMapIndex = FindMapIndexByVersion(browserVersion);

        if (browserVersionMapIndex != -1)
        {
            driverVersion = FindClosestVersionByPlatform(browserVersionMapIndex - 1, platform);
            return driverVersion is not null;
        }

        driverVersion = null;
        return false;
    }

    private static int FindMapIndexByVersion(string version)
    {
        for (int i = s_map.Length - 1; i >= 0; i--)
        {
            if (s_map[i].Version == version)
            {
                return i;
            }
        }

        return -1;
    }

    private static string FindClosestVersionByPlatform(int startIndex, OSPlatforms platform)
    {
        for (int i = startIndex; i >= 0; i--)
        {
            if ((s_map[i].OSPlatforms & platform) == platform)
            {
                return s_map[i].Version;
            }
        }

        return null;
    }
}
