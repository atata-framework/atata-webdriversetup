﻿using FluentAssertions;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests;

[TestFixture]
public class EdgeDriverSetupStrategyTests
{
    private const string TestVerion = "110.0.1587.15";

    private const string ExpectedDriverDownloadUrlBase =
        $"https://msedgedriver.azureedge.net/{TestVerion}/edgedriver_";

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
            .Should().MatchRegex(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$");

    [TestCase("100.0.1155.0", ExpectedResult = "100.0.1155.0")]
    public string GetDriverVersionCorrespondingToBrowserVersion(string version) =>
        _sut.GetDriverVersionCorrespondingToBrowserVersion(version);
}