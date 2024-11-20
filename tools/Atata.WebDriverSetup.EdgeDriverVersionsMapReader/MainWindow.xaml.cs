using System.Windows;
using System.Windows.Input;

namespace Atata.WebDriverSetup.EdgeDriverVersionsMapReader;

public partial class MainWindow : Window
{
    public MainWindow() =>
        InitializeComponent();

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        BrowserVersionTextBox.Focus();
    }

    private void OnReadDriverVersionsButtonClick(object sender, RoutedEventArgs e) =>
        RunReading();

    private void OnCopyResultToClipboardButtonClick(object sender, RoutedEventArgs e) =>
        Clipboard.SetText(ResultTextBox.Text);

    private void OnBrowserVersionTextBoxKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            RunReading();
    }

    private void RunReading()
    {
        string browserVersion = BrowserVersionTextBox.Text;

        if (!string.IsNullOrWhiteSpace(browserVersion))
        {
            ResultTextBox.Text = VersionsMapReader.Run(browserVersion);
        }
    }
}
