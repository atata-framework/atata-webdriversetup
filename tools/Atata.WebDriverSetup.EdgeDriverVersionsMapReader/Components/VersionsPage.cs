namespace Atata.WebDriverSetup.EdgeDriverVersionsMapReader;

using _ = VersionsPage;

public sealed class VersionsPage : Page<_>
{
    public ControlList<GridLink<_>, _> VersionLinks { get; private set; }

    public LoadingIndicator<_> LoadingIndicator { get; private set; }

    public Button<_> Next { get; private set; }
}
