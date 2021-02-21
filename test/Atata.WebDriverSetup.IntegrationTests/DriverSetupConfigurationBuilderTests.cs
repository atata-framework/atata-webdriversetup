using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests
{
    public class DriverSetupConfigurationBuilderTests : IntegrationTestFixture
    {
        [TestCase(BrowserNames.Chrome)]
        [TestCase(BrowserNames.Firefox)]
        [TestCase(BrowserNames.Edge, ExcludePlatform = Platforms.Linux)]
        [TestCase(BrowserNames.Opera)]
        [TestCase(BrowserNames.InternetExplorer, IncludePlatform = Platforms.Windows)]
        public void SetUp_WithAutoVersion(string browserName)
        {
            var result = DriverSetup.Configure(browserName)
                .WithAutoVersion()
                .SetUp();

            AssertDriverIsSetUp(result, browserName);
            AssertVersionCache(browserName);
        }

        [TestCase(BrowserNames.Chrome, IncludePlatform = Platforms.Windows)]
        [TestCase(BrowserNames.Firefox, IncludePlatform = Platforms.WindowsAndLinux)]
        [TestCase(BrowserNames.Edge, IncludePlatform = Platforms.Windows)]
        [TestCase(BrowserNames.Opera, IncludePlatform = Platforms.Windows)]
        [TestCase(BrowserNames.InternetExplorer, IncludePlatform = Platforms.Windows)]
        public void SetUp_WithAutoVersion_WithX32Architecture(string browserName)
        {
            var result = DriverSetup.Configure(browserName)
                .WithAutoVersion()
                .WithX32Architecture()
                .SetUp();

            AssertDriverIsSetUp(result, browserName, architecture: Architecture.X32);
            AssertVersionCache(browserName);
        }

        [TestCase(BrowserNames.Chrome)]
        [TestCase(BrowserNames.Firefox)]
        [TestCase(BrowserNames.Edge, ExcludePlatform = Platforms.Linux)]
        [TestCase(BrowserNames.Opera)]
        [TestCase(BrowserNames.InternetExplorer, IncludePlatform = Platforms.Windows)]
        public void SetUp_WithLatestVersion(string browserName)
        {
            var result = DriverSetup.Configure(browserName)
                .WithLatestVersion()
                .SetUp();

            AssertDriverIsSetUp(result, browserName);
            AssertVersionCache(browserName);
        }

        [TestCase(BrowserNames.Chrome, "87.0.4280.88")]
        [TestCase(BrowserNames.Firefox, "0.28.0")]
        [TestCase(BrowserNames.Edge, "89.0.774.4", ExcludePlatform = Platforms.Linux)]
        [TestCase(BrowserNames.Opera, "86.0.4240.80")]
        [TestCase(BrowserNames.InternetExplorer, "3.141.59", IncludePlatform = Platforms.Windows)]
        public void SetUp_WithVersion(string browserName, string version)
        {
            var result = DriverSetup.Configure(browserName)
                .WithVersion(version)
                .SetUp();

            AssertDriverIsSetUp(result, browserName, version);
        }

        [TestCase(BrowserNames.Chrome, "87")]
        [TestCase(BrowserNames.Chrome, "87.0.4280")]
        [TestCase(BrowserNames.Edge, "89.0.774.4", ExcludePlatform = Platforms.Linux)]
        public void SetUp_ByBrowserVersion(string browserName, string version)
        {
            var result = DriverSetup.Configure(browserName)
                .ByBrowserVersion(version)
                .SetUp();

            AssertDriverIsSetUp(result, browserName, version);
            AssertVersionCache(browserName, version);
        }

        [TestCase(BrowserNames.Firefox, "84")]
        [TestCase(BrowserNames.Opera, "73.0.3856.329")]
        [TestCase(BrowserNames.InternetExplorer, "11.0.0.4", IncludePlatform = Platforms.Windows)]
        public void SetUp_ByBrowserVersion_Unsupported(string browserName, string version)
        {
            var builder = DriverSetup.Configure(browserName)
                .ByBrowserVersion(version);

            Assert.Throws<DriverSetupException>(() =>
                builder.SetUp());
        }
    }
}
