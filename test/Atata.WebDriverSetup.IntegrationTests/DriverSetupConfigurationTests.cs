using FluentAssertions;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests
{
    [TestFixture]
    public class DriverSetupConfigurationTests
    {
        private DriverSetupOptions globalOptions;

        private DriverSetupOptions defaultOptions;

        private DriverSetupConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            DriverSetup.GetDefaultConfiguration(BrowserNames.InternetExplorer)
                .WithX32Architecture();

            globalOptions = new DriverSetupOptions();
            defaultOptions = new DriverSetupOptions(globalOptions);
            configuration = new DriverSetupConfiguration(defaultOptions);
        }

        [Test]
        public void SetProperty()
        {
            const string testValue = "some-path";
            configuration.StorageDirectoryPath = testValue;

            configuration.StorageDirectoryPath.Should().Be(testValue);
        }

        [Test]
        public void SetGlobalOptionsProperty()
        {
            const string testValue = "some-path";
            globalOptions.StorageDirectoryPath = testValue;

            configuration.StorageDirectoryPath.Should().Be(testValue);
        }

        [Test]
        public void SetDefaultOptionsProperty()
        {
            const string testValue = "some-path";
            defaultOptions.StorageDirectoryPath = testValue;

            configuration.StorageDirectoryPath.Should().Be(testValue);
        }

        [Test]
        public void SetGlobalOptionsProperty_ThenDefaultOptionsProperty()
        {
            const string testValue = "some-path";
            globalOptions.StorageDirectoryPath = "global-path";
            defaultOptions.StorageDirectoryPath = testValue;

            configuration.StorageDirectoryPath.Should().Be(testValue);
        }
    }
}
