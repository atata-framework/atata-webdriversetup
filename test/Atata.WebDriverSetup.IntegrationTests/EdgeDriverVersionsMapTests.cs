namespace Atata.WebDriverSetup.IntegrationTests;

[Parallelizable(ParallelScope.None)]
public abstract class EdgeDriverVersionsMapTests
{
    private protected FakeHttpRequestExecutorProxy FakeHttpRequestExecutorProxy { get; set; } = null!;

    [SetUp]
    public void SetUpTest()
    {
        FakeHttpRequestExecutorProxy = new();
        EdgeDriverVersionsMap.ResetRemoteMapCache();
    }

    [OneTimeTearDown]
    public void TearDownSuite() =>
        EdgeDriverVersionsMap.ResetRemoteMapCache();

    public sealed class GetDriverVersionCorrespondingToBrowserVersionAsync : EdgeDriverVersionsMapTests
    {
        [Test]
        public async Task WhenBrowserVersionHasDriverInLocalMap()
        {
            var result = await EdgeDriverVersionsMap.GetDriverVersionCorrespondingToBrowserVersionAsync(
                "130.0.2849.89",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().Be("130.0.2849.89");
        }

        [Test]
        public async Task WhenClosestBrowserVersionHasDriverInLocalMap()
        {
            var result = await EdgeDriverVersionsMap.GetDriverVersionCorrespondingToBrowserVersionAsync(
                "130.0.2849.89",
                OSPlatforms.Linux64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().Be("130.0.2849.78");
        }

        [Test]
        public async Task WhenBrowserVersionIsNotPresentInLocalAndRemoteMaps()
        {
            var result = await EdgeDriverVersionsMap.GetDriverVersionCorrespondingToBrowserVersionAsync(
                "999.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task WhenBrowserVersionIsNotPresentInLocalAndRemoteMaps_ButThereIsClosestByMajor()
        {
            var result = await EdgeDriverVersionsMap.GetDriverVersionCorrespondingToBrowserVersionAsync(
                "130.0.9999.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().Be("130.0.2849.142");
        }

        [Test]
        public async Task WhenBrowserVersionIsNotPresentInLocalMapAndRemoteMapFailsToDownload()
        {
            FakeHttpRequestExecutorProxy.DownloadStringInterceptions.Enqueue(
                _ => Task.FromResult(string.Empty));

            var result = await EdgeDriverVersionsMap.GetDriverVersionCorrespondingToBrowserVersionAsync(
                "130.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task WhenBrowserVersionIsNotPresentInLocalMapButPresentInRemoteMap()
        {
            FakeHttpRequestExecutorProxy.DownloadStringInterceptions.Enqueue(
                async call =>
                {
                    string result = await call.Invoke()
                        .ConfigureAwait(false);

                    int indexOfLastVersionEnd = result.LastIndexOf(',') + 1;
                    return result.Insert(
                        indexOfLastVersionEnd,
                        $"""

                                new("999.0.0.0", Windows64),
                        """);
                });

            var result = await EdgeDriverVersionsMap.GetDriverVersionCorrespondingToBrowserVersionAsync(
                "999.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().Be("999.0.0.0");
        }
    }

    public sealed class GetDriverVersionClosestToBrowserVersionAsync : EdgeDriverVersionsMapTests
    {
        [Test]
        public async Task WhenBrowserVersionHasDriverInLocalMap()
        {
            var result = await EdgeDriverVersionsMap.GetDriverVersionClosestToBrowserVersionAsync(
                "130.0.2849.89",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().Be("130.0.2849.80");
        }

        [Test]
        public async Task WhenClosestBrowserVersionHasDriverInLocalMap()
        {
            var result = await EdgeDriverVersionsMap.GetDriverVersionClosestToBrowserVersionAsync(
                "130.0.2849.89",
                OSPlatforms.Linux64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().Be("130.0.2849.78");
        }

        [Test]
        public async Task WhenBrowserVersionIsNotPresentInLocalAndRemoteMaps()
        {
            var result = await EdgeDriverVersionsMap.GetDriverVersionClosestToBrowserVersionAsync(
                "999.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task WhenBrowserVersionIsNotPresentInLocalAndRemoteMaps_ButThereIsClosestByMajor()
        {
            var result = await EdgeDriverVersionsMap.GetDriverVersionClosestToBrowserVersionAsync(
                "130.0.9999.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().Be("130.0.2849.142");
        }

        [Test]
        public async Task WhenBrowserVersionIsNotPresentInLocalMapAndRemoteMapFailsToDownload()
        {
            FakeHttpRequestExecutorProxy.DownloadStringInterceptions.Enqueue(
                _ => Task.FromResult(string.Empty));

            var result = await EdgeDriverVersionsMap.GetDriverVersionClosestToBrowserVersionAsync(
                "130.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task WhenBrowserVersionIsNotPresentInLocalMapButPresentInRemoteMap()
        {
            FakeHttpRequestExecutorProxy.DownloadStringInterceptions.Enqueue(
                async call =>
                {
                    string result = await call.Invoke()
                        .ConfigureAwait(false);

                    int indexOfLastVersionEnd = result.LastIndexOf(',') + 1;
                    return result.Insert(
                        indexOfLastVersionEnd,
                        $"""

                                new("996.0.0.0", Windows64),
                                new("997.0.0.0", Windows64),
                                new("998.0.0.0", MacArm64),
                                new("999.0.0.0", Windows64),
                        """);
                });

            var result = await EdgeDriverVersionsMap.GetDriverVersionClosestToBrowserVersionAsync(
                "999.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                CancellationToken.None);

            result.Should().Be("997.0.0.0");
        }
    }
}
