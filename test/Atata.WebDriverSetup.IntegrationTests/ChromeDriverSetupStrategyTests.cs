using FluentAssertions;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests
{
    [TestFixture]
    public class ChromeDriverSetupStrategyTests
    {
        private readonly ChromeDriverSetupStrategy _sut = new ChromeDriverSetupStrategy(
            new HttpRequestExecutor());

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
}
