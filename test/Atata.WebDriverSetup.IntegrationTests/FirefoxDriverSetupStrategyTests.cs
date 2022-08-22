using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests
{
    [TestFixture]
    public class FirefoxDriverSetupStrategyTests
    {
        [Test]
        public void GetInstalledBrowserVersion()
        {
            var sut = new FirefoxDriverSetupStrategy(
                Mock.Of<IHttpRequestExecutor>());

            string result = sut.GetInstalledBrowserVersion();

            result.Should().NotBeNullOrWhiteSpace();
            result[0].Should().BeInRange('1', '9');
        }
    }
}
