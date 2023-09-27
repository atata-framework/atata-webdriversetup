namespace Atata.WebDriverSetup.IntegrationTests;

[TestFixture]
public class ChromeDriverSetupStrategyTests
{
    private const string TestVersion = "110.0.5481.30";

    private const string ExpectedDriverDownloadUrlBase =
        $"https://chromedriver.storage.googleapis.com/{TestVersion}/chromedriver_";

    private const string ExpectedWindowsX32DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}win32.zip";

    private const string ExpectedMacX64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}mac64.zip";

    private const string ExpectedMacArm64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}mac_arm64.zip";

    private const string ExpectedLinuxX64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}linux64.zip";

    private readonly ChromeDriverSetupStrategy _sut = new(
        new HttpRequestExecutor());

    [TestCase(
        Architecture.X32,
        IncludePlatform = Platforms.Windows,
        ExpectedResult = ExpectedWindowsX32DriverDownloadUrl)]
    [TestCase(
        Architecture.X64,
        IncludePlatform = Platforms.Windows,
        ExpectedResult = ExpectedWindowsX32DriverDownloadUrl)]
    [TestCase(
        Architecture.Arm64,
        IncludePlatform = Platforms.Windows,
        ExpectedResult = ExpectedWindowsX32DriverDownloadUrl)]
    [TestCase(
        Architecture.X32,
        IncludePlatform = Platforms.MacOS,
        ExpectedResult = ExpectedMacX64DriverDownloadUrl)]
    [TestCase(
        Architecture.X64,
        IncludePlatform = Platforms.MacOS,
        ExpectedResult = ExpectedMacX64DriverDownloadUrl)]
    [TestCase(
        Architecture.Arm64,
        IncludePlatform = Platforms.MacOS,
        ExpectedResult = ExpectedMacArm64DriverDownloadUrl)]
    [TestCase(
        Architecture.X32,
        IncludePlatform = Platforms.Linux,
        ExpectedResult = ExpectedLinuxX64DriverDownloadUrl)]
    [TestCase(
        Architecture.X64,
        IncludePlatform = Platforms.Linux,
        ExpectedResult = ExpectedLinuxX64DriverDownloadUrl)]
    [TestCase(
        Architecture.Arm64,
        IncludePlatform = Platforms.Linux,
        ExpectedResult = ExpectedLinuxX64DriverDownloadUrl)]
    public string GetDriverDownloadUrl(Architecture architecture)
    {
        var url = _sut.GetDriverDownloadUrl(TestVersion, architecture);

        Assertions.AssertUrlReturnsOK(url);

        return url.ToString();
    }

    [TestCaseSource(typeof(Architectures), nameof(Architectures.All))]
    public void GetDriverDownloadUrl_WithLatestVersion(Architecture architecture)
    {
        string latestVersion = _sut.GetDriverLatestVersion();
        var url = _sut.GetDriverDownloadUrl(latestVersion, architecture);

        Assertions.AssertUrlReturnsOK(url);
    }

    [Test]
    public void GetInstalledBrowserVersion() =>
        _sut.GetInstalledBrowserVersion()
            .Should().MatchRegex(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$");

    [TestCase("101.0.4951.41", ExpectedResult = "101.0.4951.41")]
    [TestCase("97.0", ExpectedResult = "97.0.4692.71")]
    [TestCase("94", ExpectedResult = "94.0.4606.113")]
    public string GetDriverVersionCorrespondingToBrowserVersion(string version) =>
        _sut.GetDriverVersionCorrespondingToBrowserVersion(version);
}
