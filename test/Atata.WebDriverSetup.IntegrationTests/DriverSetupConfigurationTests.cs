namespace Atata.WebDriverSetup.IntegrationTests;

[TestFixture]
public class DriverSetupConfigurationTests
{
    private DriverSetupOptions _globalOptions;

    private DriverSetupOptions _defaultOptions;

    private DriverSetupConfiguration _configuration;

    [SetUp]
    public void SetUp()
    {
        _globalOptions = new DriverSetupOptions();
        _defaultOptions = new DriverSetupOptions(_globalOptions);
        _configuration = new DriverSetupConfiguration(_defaultOptions);
    }

    [Test]
    public void SetProperty()
    {
        const string testValue = "some-path";
        _configuration.StorageDirectoryPath = testValue;

        _configuration.StorageDirectoryPath.Should().Be(testValue);
    }

    [Test]
    public void SetGlobalOptionsProperty()
    {
        const string testValue = "some-path";
        _globalOptions.StorageDirectoryPath = testValue;

        _configuration.StorageDirectoryPath.Should().Be(testValue);
    }

    [Test]
    public void SetDefaultOptionsProperty()
    {
        const string testValue = "some-path";
        _defaultOptions.StorageDirectoryPath = testValue;

        _configuration.StorageDirectoryPath.Should().Be(testValue);
    }

    [Test]
    public void SetGlobalOptionsProperty_ThenDefaultOptionsProperty()
    {
        const string testValue = "some-path";
        _globalOptions.StorageDirectoryPath = "global-path";
        _defaultOptions.StorageDirectoryPath = testValue;

        _configuration.StorageDirectoryPath.Should().Be(testValue);
    }
}
