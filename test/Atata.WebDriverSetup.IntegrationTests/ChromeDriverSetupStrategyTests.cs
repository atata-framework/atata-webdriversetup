using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests
{
    [TestFixture]
    public class ChromeDriverSetupStrategyTests
    {
        [Test]
        public void GetInstalledBrowserVersion()
        {
            var sut = new ChromeDriverSetupStrategy(
                Mock.Of<IHttpRequestExecutor>());

            string result = sut.GetInstalledBrowserVersion();

            result.Should().MatchRegex(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$");
        }
    }
}
