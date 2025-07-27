namespace Atata.WebDriverSetup.IntegrationTests;

public static class BrowserDetectorTests
{
    [TestFixture]
    public class GetFirstInstalledBrowserNameAsync
    {
        [Test]
        public async Task With3ValidBrowserNames()
        {
            string[] browserNames = [BrowserNames.Chrome, BrowserNames.Firefox, BrowserNames.Edge];
            var result = await BrowserDetector.GetFirstInstalledBrowserNameAsync(browserNames);

            result.Should().BeOneOf(browserNames);
        }

        [Test]
        public void WithNull() =>
            Assert.That(
                async () => await BrowserDetector.GetFirstInstalledBrowserNameAsync(null!),
                Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void WithEmptyEnumerable() =>
            Assert.That(
                async () => await BrowserDetector.GetFirstInstalledBrowserNameAsync([]),
                Throws.TypeOf<ArgumentException>());

        [Test]
        public async Task WithInvalidBrowserName()
        {
            var result = await BrowserDetector.GetFirstInstalledBrowserNameAsync(["InvalidName"]);

            result.Should().BeNull();
        }
    }
}
