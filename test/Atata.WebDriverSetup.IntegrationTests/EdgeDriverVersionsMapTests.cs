namespace Atata.WebDriverSetup.IntegrationTests;

[Parallelizable(ParallelScope.None)]
public abstract class EdgeDriverVersionsMapTests
{
    private protected FakeHttpRequestExecutorProxy FakeHttpRequestExecutorProxy { get; set; }

    [SetUp]
    public void SetUpTest()
    {
        FakeHttpRequestExecutorProxy = new();
        EdgeDriverVersionsMap.ResetRemoteMapCache();
    }

    [OneTimeTearDown]
    public void TearDownSuite() =>
        EdgeDriverVersionsMap.ResetRemoteMapCache();

    public sealed class TryGetDriverVersionCorrespondingToBrowserVersion : EdgeDriverVersionsMapTests
    {
        [Test]
        public void WhenBrowserVersionHasDriverInLocalMap()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
                "130.0.2849.89",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.89");
        }

        [Test]
        public void WhenClosestBrowserVersionHasDriverInLocalMap()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
                "130.0.2849.89",
                OSPlatforms.Linux64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.78");
        }

        [Test]
        public void WhenBrowserVersionIsNotPresentInLocalAndRemoteMaps()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
                "999.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeFalse();
            driverVersion.Should().BeNull();
        }

        [Test]
        public void WhenBrowserVersionIsNotPresentInLocalAndRemoteMaps_ButThereIsClosestByMajor()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
                "130.0.9999.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.89");
        }

        [Test]
        public void WhenBrowserVersionIsNotPresentInLocalMapAndRemoteMapFailsToDownload()
        {
            FakeHttpRequestExecutorProxy.DownloadStringInterceptions.Enqueue(_ => string.Empty);

            bool result = EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
                "130.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeFalse();
            driverVersion.Should().BeNull();
        }

        [Test]
        public void WhenBrowserVersionIsNotPresentInLocalMapButPresentInRemoteMap()
        {
            FakeHttpRequestExecutorProxy.DownloadStringInterceptions.Enqueue(
                call =>
                {
                    string result = call.Invoke();

                    int indexOfLastVersionEnd = result.LastIndexOf(',') + 1;
                    return result.Insert(
                        indexOfLastVersionEnd,
                        $"""

                                new("999.0.0.0", Windows64),
                        """);
                });

            bool result = EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
                "999.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("999.0.0.0");
        }
    }

    public sealed class TryGetDriverVersionClosestToBrowserVersion : EdgeDriverVersionsMapTests
    {
        [Test]
        public void WhenBrowserVersionHasDriverInLocalMap()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(
                "130.0.2849.89",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.80");
        }

        [Test]
        public void WhenClosestBrowserVersionHasDriverInLocalMap()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(
                "130.0.2849.89",
                OSPlatforms.Linux64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.78");
        }

        [Test]
        public void WhenBrowserVersionIsNotPresentInLocalAndRemoteMaps()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(
                "999.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeFalse();
            driverVersion.Should().BeNull();
        }

        [Test]
        public void WhenBrowserVersionIsNotPresentInLocalAndRemoteMaps_ButThereIsClosestByMajor()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(
                "130.0.9999.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.89");
        }

        [Test]
        public void WhenBrowserVersionIsNotPresentInLocalMapAndRemoteMapFailsToDownload()
        {
            FakeHttpRequestExecutorProxy.DownloadStringInterceptions.Enqueue(_ => string.Empty);

            bool result = EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(
                "130.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeFalse();
            driverVersion.Should().BeNull();
        }

        [Test]
        public void WhenBrowserVersionIsNotPresentInLocalMapButPresentInRemoteMap()
        {
            FakeHttpRequestExecutorProxy.DownloadStringInterceptions.Enqueue(
                call =>
                {
                    string result = call.Invoke();

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

            bool result = EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(
                "999.0.0.0",
                OSPlatforms.Windows64,
                FakeHttpRequestExecutorProxy,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("997.0.0.0");
        }
    }
}
