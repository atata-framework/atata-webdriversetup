using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests;

[TestFixture]
public class FirefoxDriverSetupStrategyTests
{
    private readonly FirefoxDriverSetupStrategy _sut = new(
        Mock.Of<IHttpRequestExecutor>());

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
