using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests;

[TestFixture]
public class EdgeDriverSetupStrategyTests
{
    private readonly EdgeDriverSetupStrategy _sut = new(
        Mock.Of<IHttpRequestExecutor>());

    [Test]
    public void GetInstalledBrowserVersion() =>
        _sut.GetInstalledBrowserVersion()
            .Should().MatchRegex(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$");

    [TestCase("100.0.1155.0", ExpectedResult = "100.0.1155.0")]
    public string GetDriverVersionCorrespondingToBrowserVersion(string version) =>
        _sut.GetDriverVersionCorrespondingToBrowserVersion(version);
}
