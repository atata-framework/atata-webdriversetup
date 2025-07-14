namespace Atata.WebDriverSetup.EdgeDriverVersionsMapReader;

using _ = VersionPage;

public sealed class VersionPage : Page<_>
{
    public ControlList<GridLink<_>, _> DriverLinks { get; private set; } = null!;

    public LoadingIndicator<_> LoadingIndicator { get; private set; } = null!;
}
