namespace Atata.WebDriverSetup.IntegrationTests;

public class DriverSetupConfigurationBuilderTests : IntegrationTestFixture
{
    [TestCase(BrowserNames.Chrome)]
    [TestCase(BrowserNames.Firefox)]
    [TestCase(BrowserNames.Edge)]
    [TestCase(BrowserNames.Opera)]
    [TestCase(BrowserNames.InternetExplorer, IncludePlatform = Platforms.Windows)]
    public async Task SetUpAsync_WithAutoVersion(string browserName)
    {
        var result = await DriverSetup.Configure(browserName)
            .WithAutoVersion()
            .SetUpAsync();

        AssertDriverIsSetUp(result, browserName);
        AssertVersionCache(browserName);
    }

    [TestCase(BrowserNames.Chrome, IncludePlatform = Platforms.Windows)]
    [TestCase(BrowserNames.Firefox, IncludePlatform = Platforms.WindowsAndLinux)]
    [TestCase(BrowserNames.Edge, IncludePlatform = Platforms.Windows)]
    [TestCase(BrowserNames.Opera, IncludePlatform = Platforms.Windows)]
    [TestCase(BrowserNames.InternetExplorer, IncludePlatform = Platforms.Windows)]
    public async Task SetUpAsync_WithAutoVersion_WithX32Architecture(string browserName)
    {
        var result = await DriverSetup.Configure(browserName)
            .WithAutoVersion()
            .WithX32Architecture()
            .SetUpAsync();

        AssertDriverIsSetUp(result, browserName, architecture: Architecture.X32);
        AssertVersionCache(browserName);
    }

    [TestCase(BrowserNames.Chrome)]
    [TestCase(BrowserNames.Firefox)]
    [TestCase(BrowserNames.Edge, IncludePlatform = Platforms.Windows)]
    [TestCase(BrowserNames.Opera)]
    [TestCase(BrowserNames.InternetExplorer, IncludePlatform = Platforms.Windows)]
    public async Task SetUpAsync_WithLatestVersion(string browserName)
    {
        var result = await DriverSetup.Configure(browserName)
            .WithLatestVersion()
            .SetUpAsync();

        AssertDriverIsSetUp(result, browserName);
        AssertVersionCache(browserName);
    }

    [TestCase(BrowserNames.Chrome, "87.0.4280.88")]
    [TestCase(BrowserNames.Chrome, "114.0.5735.90")]
    [TestCase(BrowserNames.Chrome, "115.0.5790.98")]
    [TestCase(BrowserNames.Chrome, "121.0.6167.57")]
    [TestCase(BrowserNames.Chrome, "121.0.6167.85")]
    [TestCase(BrowserNames.Firefox, "0.28.0")]
    [TestCase(BrowserNames.Edge, "135.0.3179.54")]
    [TestCase(BrowserNames.Opera, "86.0.4240.80")]
    [TestCase(BrowserNames.InternetExplorer, "3.141.59", IncludePlatform = Platforms.Windows)]
    [TestCase(BrowserNames.InternetExplorer, "4.11.0", IncludePlatform = Platforms.Windows)]
    public async Task SetUpAsync_WithVersion(string browserName, string version)
    {
        var result = await DriverSetup.Configure(browserName)
            .WithVersion(version)
            .SetUpAsync();

        AssertDriverIsSetUp(result, browserName, version);
    }

    [TestCase(BrowserNames.Chrome, "87", "87")]
    [TestCase(BrowserNames.Chrome, "87.0.4280", "87.0.4280")]
    [TestCase(BrowserNames.Chrome, "114", "114")]
    [TestCase(BrowserNames.Chrome, "115", "115")]
    [TestCase(BrowserNames.Chrome, "117.0.5896", "117.0.5896.0")]
    [TestCase(BrowserNames.Firefox, "101", "0.31.0")]
    [TestCase(BrowserNames.Edge, "135.0.3179.54", "135.0.3179.54")]
    public async Task SetUpAsync_ByBrowserVersion(string browserName, string browserVersion, string driverVersion)
    {
        var result = await DriverSetup.Configure(browserName)
            .ByBrowserVersion(browserVersion)
            .SetUpAsync();

        AssertDriverIsSetUp(result, browserName, driverVersion);
        AssertVersionCache(browserName, browserVersion);
    }

    [TestCase(BrowserNames.Opera, "73.0.3856.329")]
    [TestCase(BrowserNames.InternetExplorer, "11.0.0.4", IncludePlatform = Platforms.Windows)]
    public void SetUpAsync_ByBrowserVersion_Unsupported(string browserName, string version)
    {
        var builder = DriverSetup.Configure(browserName)
            .ByBrowserVersion(version);

        Assert.That(
            async () => await builder.SetUpAsync(),
            Throws.TypeOf<DriverSetupException>());
    }

    [Test]
    public async Task SetUpAsync_Edge_WithAutoVersion_WhichFailsToDownloadAndClosestVersionIsDownloaded() =>
        await TestEdgeSetUpAsyncWhichFailsToDownloadAndPreviousVersionIsDownloadedAsync(x => x.WithAutoVersion());

    [Test]
    public async Task SetUpAsync_Edge_WithLatestVersion_WhichFailsToDownloadAndClosestVersionIsDownloaded() =>
        await TestEdgeSetUpAsyncWhichFailsToDownloadAndPreviousVersionIsDownloadedAsync(x => x.WithLatestVersion());

    private static async Task TestEdgeSetUpAsyncWhichFailsToDownloadAndPreviousVersionIsDownloadedAsync(
        Action<DriverSetupConfigurationBuilder> builderConfiguration)
    {
        FakeHttpRequestExecutorProxy fakeHttpRequestExecutorProxy = null!;

        var builder = DriverSetup.Configure(BrowserNames.Edge);

        builderConfiguration.Invoke(builder);

        var result = await builder
            .WithCheckCertificateRevocationList(false)
            .WithHttpRequestTryCount(1)
            .WithHttpRequestExecutor(
                config => fakeHttpRequestExecutorProxy = new FakeHttpRequestExecutorProxy(
                    DriverSetupConfigurationBuilder.CreateDefaultHttpRequestExecutor(config))
                {
                    DownloadFileFailuresCount = 1
                })
            .SetUpAsync();

        AssertDriverIsSetUp(result, BrowserNames.Edge);
        AssertVersionCache(BrowserNames.Edge);
        fakeHttpRequestExecutorProxy.DownloadFileCalls.Count.Should().Be(2);
        fakeHttpRequestExecutorProxy.DownloadFileCalls[1].Should().NotBe(
            fakeHttpRequestExecutorProxy.DownloadFileCalls[0]);
    }
}
