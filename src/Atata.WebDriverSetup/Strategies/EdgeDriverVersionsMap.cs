namespace Atata.WebDriverSetup;

internal static partial class EdgeDriverVersionsMap
{
    private const string RemoteMapUrl = "https://raw.githubusercontent.com/atata-framework/atata-webdriversetup/refs/heads/main/src/Atata.WebDriverSetup/Strategies/EdgeDriverVersionsMap.Values.cs";

    private static RemoteMapState s_remoteMapState;

    private static string? s_remoteMapText;

    private enum RemoteMapState
    {
        NotDownloaded,
        DownloadFailed,
        Same,
        Fresher
    }

    internal static async Task<string?> GetDriverVersionCorrespondingToBrowserVersionAsync(
        string browserVersion,
        OSPlatforms platform,
        IHttpRequestExecutor httpRequestExecutor,
        CancellationToken cancellationToken)
    {
        int browserVersionMapIndex = FindMapIndexByVersion(browserVersion);
        string? driverVersion;

        if (browserVersionMapIndex != -1)
        {
            driverVersion = FindClosestVersionInMapByPlatform(browserVersionMapIndex, platform);
            if (driverVersion is not null)
                return driverVersion;
        }

        if (await EnsureRemoteMapDownloadedAndFresherAsync(httpRequestExecutor, cancellationToken).ConfigureAwait(false))
        {
            int browserVersionRemoteMapTextIndex = FindRemoteMapTextIndexByVersion(browserVersion);

            if (browserVersionRemoteMapTextIndex != -1)
            {
                int browserVersionEndRemoteMapTextIndex = FindRemoteMapTextLineEndIndex(browserVersionRemoteMapTextIndex);

                driverVersion = FindClosestVersionInRemoteMapTextByPlatform(browserVersionEndRemoteMapTextIndex, platform);
                if (driverVersion is not null)
                    return driverVersion;
            }
        }

        if (TryGetDriverVersionClosestToBrowserVersionByMajorVersionNumber(browserVersion, platform, out driverVersion))
            return driverVersion;

        return null;
    }

    internal static async Task<string?> GetDriverVersionClosestToBrowserVersionAsync(
        string browserVersion,
        OSPlatforms platform,
        IHttpRequestExecutor httpRequestExecutor,
        CancellationToken cancellationToken)
    {
        int browserVersionMapIndex = FindMapIndexByVersion(browserVersion);
        string? driverVersion;

        if (browserVersionMapIndex != -1)
        {
            driverVersion = FindClosestVersionInMapByPlatform(browserVersionMapIndex - 1, platform);
            if (driverVersion is not null)
                return driverVersion;
        }

        if (await EnsureRemoteMapDownloadedAndFresherAsync(httpRequestExecutor, cancellationToken).ConfigureAwait(false))
        {
            int browserVersionRemoteMapTextIndex = FindRemoteMapTextIndexByVersion(browserVersion);

            if (browserVersionRemoteMapTextIndex != -1)
            {
                driverVersion = FindClosestVersionInRemoteMapTextByPlatform(browserVersionRemoteMapTextIndex, platform);
                if (driverVersion is not null)
                    return driverVersion;
            }
        }

        if (TryGetDriverVersionClosestToBrowserVersionByMajorVersionNumber(browserVersion, platform, out driverVersion))
            return driverVersion;

        return null;
    }

    internal static void ResetRemoteMapCache()
    {
        s_remoteMapState = RemoteMapState.NotDownloaded;
        s_remoteMapText = null;
    }

    private static bool TryGetDriverVersionClosestToBrowserVersionByMajorVersionNumber(
        string browserVersion,
        OSPlatforms platform,
        [NotNullWhen(true)] out string? driverVersion)
    {
        if (s_remoteMapState is RemoteMapState.Same or RemoteMapState.Fresher)
        {
            int browserVersionRemoteMapTextIndex = FindRemoteMapTextIndexByMajorVersionNumber(browserVersion);

            if (browserVersionRemoteMapTextIndex != -1)
            {
                int browserVersionEndRemoteMapTextIndex = FindRemoteMapTextLineEndIndex(browserVersionRemoteMapTextIndex);

                driverVersion = FindClosestVersionInRemoteMapTextByPlatform(browserVersionEndRemoteMapTextIndex, platform);
                if (driverVersion is not null)
                    return true;
            }
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

    private static string? FindClosestVersionInMapByPlatform(int startIndex, OSPlatforms platform)
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

    private static async Task<bool> EnsureRemoteMapDownloadedAndFresherAsync(
        IHttpRequestExecutor httpRequestExecutor,
        CancellationToken cancellationToken)
    {
        await EnsureRemoteMapDownloadedAsync(httpRequestExecutor, cancellationToken)
            .ConfigureAwait(false);

        return s_remoteMapState == RemoteMapState.Fresher;
    }

    private static async Task EnsureRemoteMapDownloadedAsync(IHttpRequestExecutor httpRequestExecutor, CancellationToken cancellationToken)
    {
        if (s_remoteMapState == RemoteMapState.NotDownloaded)
        {
            try
            {
                s_remoteMapText = await httpRequestExecutor.DownloadStringAsync(RemoteMapUrl, cancellationToken)
                    .ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(s_remoteMapText))
                {
                    s_remoteMapState = RemoteMapState.DownloadFailed;
                    return;
                }

                s_remoteMapState = IsRemoteMapFresher() ? RemoteMapState.Fresher : RemoteMapState.Same;
            }
            catch (Exception exception)
            {
                Log.Warn(exception, $"Failed to download \"{RemoteMapUrl}\" or response is not OK.");
                s_remoteMapState = RemoteMapState.DownloadFailed;
            }
        }
    }

    private static bool IsRemoteMapFresher()
    {
        int lastVersionStartIndex = s_remoteMapText!.LastIndexOf("new(", StringComparison.Ordinal) + 5;
        int lastVersionEndIndex = s_remoteMapText.IndexOf('"', lastVersionStartIndex);
        string lastVersion = s_remoteMapText[lastVersionStartIndex..lastVersionEndIndex];

        return lastVersion != s_map[^1].Version;
    }

    private static int FindRemoteMapTextIndexByVersion(string version) =>
        s_remoteMapText!.LastIndexOf($"\"{version}\"", StringComparison.Ordinal);

    private static int FindRemoteMapTextIndexByMajorVersionNumber(string version)
    {
        int majorVersionNumber = VersionUtils.GetMajorNumber(version);

        return s_remoteMapText!.LastIndexOf($"\"{majorVersionNumber}.", StringComparison.Ordinal);
    }

    private static int FindRemoteMapTextLineEndIndex(int index) =>
       s_remoteMapText!.IndexOf(')', index);

    private static string? FindClosestVersionInRemoteMapTextByPlatform(int startIndex, OSPlatforms platform)
    {
        int indexOfPlatform = s_remoteMapText!.LastIndexOf(platform.ToString(), startIndex, StringComparison.Ordinal);

        if (indexOfPlatform != -1)
        {
            int versionEndIndex = s_remoteMapText.LastIndexOf('"', indexOfPlatform);
            int versionStartIndex = s_remoteMapText.LastIndexOf('"', versionEndIndex - 1) + 1;
            return s_remoteMapText[versionStartIndex..versionEndIndex];
        }

        return null;
    }
}
