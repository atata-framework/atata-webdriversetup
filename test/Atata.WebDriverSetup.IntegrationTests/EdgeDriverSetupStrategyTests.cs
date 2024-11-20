namespace Atata.WebDriverSetup.IntegrationTests;

[TestFixture]
public class EdgeDriverSetupStrategyTests
{
    private const string TestVersion = "116.0.1938.81";

    private const string ExpectedDriverDownloadUrlBase =
        $"https://msedgedriver.azureedge.net/{TestVersion}/edgedriver_";

    private const string ExpectedWindowsX32DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}win32.zip";

    private const string ExpectedWindowsX64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}win64.zip";

    private const string ExpectedWindowsArm64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}arm64.zip";

    private const string ExpectedMacX64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}mac64.zip";

    private const string ExpectedMacArm64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}mac64_m1.zip";

    private const string ExpectedLinuxX64DriverDownloadUrl =
        $"{ExpectedDriverDownloadUrlBase}linux64.zip";

    private readonly EdgeDriverSetupStrategy _sut = new(
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
        var url = _sut.GetDriverDownloadUrl(TestVersion, TargetOSPlatform.Detect(architecture));

        Assertions.AssertUrlReturnsOK(url);

        return url.ToString();
    }

    [Test]
    public void GetInstalledBrowserVersion() =>
        _sut.GetInstalledBrowserVersion()
            .Should().MatchRegex(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$");

    [TestCase("100.0.1155.0", ExpectedResult = "100.0.1155.0")]
    [TestCase("150.1.2.3", ExpectedResult = "150.1.2.3")]
    [TestCase("130.0.2816.0", ExpectedResult = "130.0.2816.0")]
    [TestCase("130.0.2849.50", IncludePlatform = Platforms.WindowsAndLinux, ExpectedResult = "130.0.2849.46")]
    [TestCase("130.0.2841.0", IncludePlatform = Platforms.MacOS, ExpectedResult = "130.0.2821.1")]
    public string GetDriverVersionCorrespondingToBrowserVersion(string version) =>
        _sut.GetDriverVersionCorrespondingToBrowserVersion(version, TargetOSPlatform.DetectAuto());
}
