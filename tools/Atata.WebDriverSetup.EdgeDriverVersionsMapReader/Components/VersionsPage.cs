namespace Atata.WebDriverSetup.EdgeDriverVersionsMapReader;

using _ = VersionsPage;

public sealed class VersionsPage : Page<_>
{
    public ControlList<GridLink<_>, _> VersionLinks { get; private set; } = null!;

    public LoadingIndicator<_> LoadingIndicator { get; private set; } = null!;

    public Button<_> Next { get; private set; } = null!;
}
