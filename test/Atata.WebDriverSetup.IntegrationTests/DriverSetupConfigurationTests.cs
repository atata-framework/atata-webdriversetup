namespace Atata.WebDriverSetup.IntegrationTests;

public sealed class DriverSetupConfigurationTests
{
    private DriverSetupOptions _globalOptions = null!;

    private DriverSetupOptions _defaultOptions = null!;

    private DriverSetupConfiguration _configuration = null!;

    [SetUp]
    public void SetUp()
    {
        _globalOptions = new();
        _defaultOptions = new(_globalOptions);
        _configuration = new(_defaultOptions);
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
