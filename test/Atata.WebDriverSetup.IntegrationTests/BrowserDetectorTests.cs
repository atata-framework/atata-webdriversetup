namespace Atata.WebDriverSetup.IntegrationTests;

public static class BrowserDetectorTests
{
    [TestFixture]
    public class GetFirstInstalledBrowserName
    {
        [Test]
        public void With3ValidBrowserNames()
        {
            var browserNames = new[] { BrowserNames.Chrome, BrowserNames.Firefox, BrowserNames.Edge };
            var result = BrowserDetector.GetFirstInstalledBrowserName(browserNames);

            result.Should().BeOneOf(browserNames);
        }

        [Test]
        public void WithNull() =>
            Assert.Throws<ArgumentNullException>(() =>
                BrowserDetector.GetFirstInstalledBrowserName((null as IEnumerable<string>)!));

        [Test]
        public void WithEmptyEnumerable() =>
            Assert.Throws<ArgumentException>(() =>
                BrowserDetector.GetFirstInstalledBrowserName());

        [Test]
        public void WithInvalidBrowserName() =>
            BrowserDetector.GetFirstInstalledBrowserName("InvalidName")
                .Should().BeNull();
    }
}
