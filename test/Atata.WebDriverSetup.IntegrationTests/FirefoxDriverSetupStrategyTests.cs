namespace Atata.WebDriverSetup.IntegrationTests;

[TestFixture]
public class FirefoxDriverSetupStrategyTests
{
    private const string TestVerion = "0.32.0";

    private const string ExpectedDriverDownloadUrlBase =
        $"https://github.com/mozilla/geckodriver/releases/download/v{TestVerion}/geckodriver-v{TestVerion}-";

    private const string ExpectedWindowsX32DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}win32.zip";

    private const string ExpectedWindowsX64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}win64.zip";

    private const string ExpectedWindowsArm64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}win-aarch64.zip";

    private const string ExpectedMacDriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}macos.tar.gz";

    private const string ExpectedMacArm64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}macos-aarch64.tar.gz";

    private const string ExpectedLinuxX32DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}linux32.tar.gz";

    private const string ExpectedLinuxX64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}linux64.tar.gz";

    private const string ExpectedLinuxArm64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}linux-aarch64.tar.gz";

    private readonly FirefoxDriverSetupStrategy _sut = new(
        new HttpRequestExecutor());

    [TestCase(
        Architecture.X32,
        IncludePlatform = Platforms.Windows,
        ExpectedResult = ExpectedWindowsX32DriverDownloadUrl)]
    [TestCase(
        Architecture.X64,
        IncludePlatform = Platforms.Windows,
        ExpectedResult = ExpectedWindowsX64DriverDownloadUrl)]
    [TestCase(
        Architecture.Arm64,
        IncludePlatform = Platforms.Windows,
        ExpectedResult = ExpectedWindowsArm64DriverDownloadUrl)]
    [TestCase(
        Architecture.X32,
        IncludePlatform = Platforms.MacOS,
        ExpectedResult = ExpectedMacDriverDownloadUrl)]
    [TestCase(
        Architecture.X64,
        IncludePlatform = Platforms.MacOS,
        ExpectedResult = ExpectedMacDriverDownloadUrl)]
    [TestCase(
        Architecture.Arm64,
        IncludePlatform = Platforms.MacOS,
        ExpectedResult = ExpectedMacArm64DriverDownloadUrl)]
    [TestCase(
        Architecture.X32,
        IncludePlatform = Platforms.Linux,
        ExpectedResult = ExpectedLinuxX32DriverDownloadUrl)]
    [TestCase(
        Architecture.X64,
        IncludePlatform = Platforms.Linux,
        ExpectedResult = ExpectedLinuxX64DriverDownloadUrl)]
    [TestCase(
        Architecture.Arm64,
        IncludePlatform = Platforms.Linux,
        ExpectedResult = ExpectedLinuxArm64DriverDownloadUrl)]
    public string GetDriverDownloadUrl(Architecture architecture)
    {
        var url = _sut.GetDriverDownloadUrl(TestVerion, architecture);

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
            .Should().MatchRegex(@"^[0-9]+\.[0-9]+");

    [TestCase("100.0.0", ExpectedResult = "0.31.0")]
    [TestCase("80.0-beta", ExpectedResult = "0.30.0")]
    [TestCase("75.0.0beta2", ExpectedResult = "0.29.1")]
    public string GetDriverVersionCorrespondingToBrowserVersion(string version) =>
        _sut.GetDriverVersionCorrespondingToBrowserVersion(version);
}
