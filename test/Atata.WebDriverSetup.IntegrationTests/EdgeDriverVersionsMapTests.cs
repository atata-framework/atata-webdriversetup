namespace Atata.WebDriverSetup.IntegrationTests;

public static class EdgeDriverVersionsMapTests
{
    public sealed class TryGetDriverVersionCorrespondingToBrowserVersion
    {
        [Test]
        public void WhenBrowserVersionHasDriver()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
                "130.0.2849.89",
                OSPlatforms.Windows64,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.89");
        }

        [Test]
        public void WhenClosestBrowserVersionHasDriver()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
                "130.0.2849.89",
                OSPlatforms.Linux64,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.78");
        }

        [Test]
        public void WhenBrowserVersionNotPresent()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionCorrespondingToBrowserVersion(
                "130.0.0.0",
                OSPlatforms.Windows64,
                out string driverVersion);

            result.Should().BeFalse();
            driverVersion.Should().BeNull();
        }
    }

    public sealed class TryGetDriverVersionClosestToBrowserVersion
    {
        [Test]
        public void WhenBrowserVersionHasDriver()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(
                "130.0.2849.89",
                OSPlatforms.Windows64,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.80");
        }

        [Test]
        public void WhenClosestBrowserVersionHasDriver()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(
                "130.0.2849.89",
                OSPlatforms.Linux64,
                out string driverVersion);

            result.Should().BeTrue();
            driverVersion.Should().Be("130.0.2849.78");
        }

        [Test]
        public void WhenBrowserVersionNotPresent()
        {
            bool result = EdgeDriverVersionsMap.TryGetDriverVersionClosestToBrowserVersion(
                "130.0.0.0",
                OSPlatforms.Windows64,
                out string driverVersion);

            result.Should().BeFalse();
            driverVersion.Should().BeNull();
        }
    }
}
