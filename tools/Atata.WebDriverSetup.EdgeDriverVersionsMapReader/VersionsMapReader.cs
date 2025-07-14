using System.Collections.Immutable;
using System.Text;

namespace Atata.WebDriverSetup.EdgeDriverVersionsMapReader;

internal static class VersionsMapReader
{
    private const string DriverStorageUrl = "https://msedgewebdriverstorage.z22.web.core.windows.net/";

    private static readonly (string DriverName, string PlatformName)[] s_driverPlatformMap = [
        new("edgedriver_win32.zip", "Windows32"),
        new("edgedriver_win64.zip", "Windows64"),
        new("edgedriver_arm64.zip", "WindowsArm64"),
        new("edgedriver_mac64.zip", "Mac64"),
        new("edgedriver_mac64_m1.zip", "MacArm64"),
        new("edgedriver_linux64.zip", "Linux64"),
    ];

    internal static string Run(string browserVersion)
    {
        DriverSetup.AutoSetUp(BrowserNames.Chrome);

        var contextBuilder = AtataContext.CreateDefaultNonScopedBuilder()
            .Sessions.AddWebDriver(x => x
                .UseChrome(x => x
                    .WithArguments("disable-search-engine-choice-screen", "headless=new")))
            .LogConsumers.AddNLogFile();

        using (contextBuilder.Build())
        {
            var versionsPage = Go.To<VersionsPage>(url: $"{DriverStorageUrl}?prefix={browserVersion}");

            return ReadVersionsText(versionsPage);
        }
    }

    private static string ReadVersionsText(VersionsPage versionsPage)
    {
        List<VersionLine> versionLines = [];

        foreach (var versionLine in ReadVersionLines(versionsPage))
            versionLines.Add(versionLine);

        var orderedVersionLineTexts = versionLines
            .OrderBy(x => x.Version)
            .Select(x => x.Text)
            .ToArray();

        return string.Join(Environment.NewLine, orderedVersionLineTexts);
    }

    private static IEnumerable<VersionLine> ReadVersionLines(VersionsPage versionsPage)
    {
        while (true)
        {
            foreach (var versionLine in ReadVersionLinesFromPage(versionsPage))
                yield return versionLine;

            if (versionsPage.Next.IsEnabled)
                versionsPage = versionsPage.Next.ClickAndGo<VersionsPage>();
            else
                yield break;
        }
    }

    private static IEnumerable<VersionLine> ReadVersionLinesFromPage(VersionsPage versionsPage)
    {
        foreach (var versionLink in versionsPage.VersionLinks.Skip(1).Value)
        {
            string versionString = versionLink.Content.Value.TrimEnd('/');
            Version version = new(versionString);

            versionLink.CtrlClick();

            var versionPage = Go.ToNextWindow<VersionPage>();

            string versionLine = ReadVersionLine(versionPage, versionString);

            versionPage.CloseWindow();

            yield return new(version, versionLine);
        }
    }

    private static string ReadVersionLine(VersionPage versionPage, string version)
    {
        StringBuilder resultBuilder = new();

        resultBuilder.Append(' ', 8)
            .Append("new(\"")
            .Append(version)
            .Append("\", ");

        string[] driverNames = versionPage.DriverLinks
            .Skip(1)
            .Select(x => x.Content.Value)
            .ToArray();

        bool addJoinOperator = false;

        foreach (var item in s_driverPlatformMap)
        {
            if (driverNames.Contains(item.DriverName))
            {
                if (addJoinOperator)
                    resultBuilder.Append(" | ");

                resultBuilder.Append(item.PlatformName);

                addJoinOperator = true;
            }
        }

        resultBuilder.Append("),");
        return resultBuilder.ToString();
    }

    internal sealed record class VersionLine(Version Version, string Text);
}
