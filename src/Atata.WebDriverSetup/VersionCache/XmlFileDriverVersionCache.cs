using System.Xml.Linq;
using System.Xml.XPath;

namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the driver version cache that uses XML file to store the data.
/// </summary>
public class XmlFileDriverVersionCache : IDriverVersionCache
{
    private const string RootElementName = "versions";

    private const string ItemElementName = "version";

    private const string BrowserAttributeName = "browser";

    private const string DriverAttributeName = "driver";

    private const string TimestampAttributeName = "timestamp";

    private const string TimestampFormat = "u";

    private readonly string _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlFileDriverVersionCache"/> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public XmlFileDriverVersionCache(string filePath) =>
        _filePath = filePath;

    /// <inheritdoc/>
    public async ValueTask<string> GetOrAddLatestAsync(
        DateTime minimumAcceptableTimestamp,
        Func<CancellationToken, Task<string>> latestVersionResolveFunction,
        CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNull(latestVersionResolveFunction);

        return await GetOrAddAsync(
            DriverVersions.Latest,
            minimumAcceptableTimestamp,
            (_, ct) => latestVersionResolveFunction.Invoke(ct),
            cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<string> GetOrAddAsync(
        string browserVersion,
        DateTime minimumAcceptableTimestamp,
        Func<string, CancellationToken, Task<string>> versionResolveFunction,
        CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrWhitespace(browserVersion);
        Guard.ThrowIfNull(versionResolveFunction);

        XDocument document = OpenDocument();

        XElement? item = document.XPathSelectElement(
            $"//{RootElementName}/{ItemElementName}[@{BrowserAttributeName}='{browserVersion}']");

        string? driverVersion;

        if (item is not null)
        {
            if (TryGetDriverVersion(item, minimumAcceptableTimestamp, out driverVersion))
                return driverVersion;
        }
        else
        {
            item = new XElement(ItemElementName, new XAttribute(BrowserAttributeName, browserVersion));
            document.Root!.Add(item);
        }

        driverVersion = await versionResolveFunction.Invoke(browserVersion, cancellationToken)
            .ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(driverVersion))
        {
            item.SetAttributeValue(DriverAttributeName, driverVersion);
            item.SetAttributeValue(TimestampAttributeName, DateTime.UtcNow.ToString(TimestampFormat, CultureInfo.InvariantCulture));

            document.Save(_filePath);
            return driverVersion!;
        }
        else
        {
            return null!;
        }
    }

    private static bool TryGetDriverVersion(XElement item, DateTime minimumAcceptableTimestamp, [NotNullWhen(true)] out string? version)
    {
        string? timestampAsString = item.Attribute(TimestampAttributeName)?.Value;

        string? driverVersion = item.Attribute(DriverAttributeName)?.Value;

        if (timestampAsString?.Length > 0 && driverVersion?.Length > 0)
        {
            DateTime timestamp = DateTime.ParseExact(timestampAsString, TimestampFormat, CultureInfo.InvariantCulture);

            if (timestamp >= minimumAcceptableTimestamp)
            {
                version = driverVersion;
                return true;
            }
        }

        version = null;
        return false;
    }

    private XDocument OpenDocument()
    {
        if (File.Exists(_filePath))
        {
            XDocument document = XDocument.Load(_filePath);

            if (document.Root?.Name == RootElementName)
                return document;
        }
        else
        {
            string directoryPath = Path.GetDirectoryName(_filePath)!;
            Directory.CreateDirectory(directoryPath);
        }

        return new(
            new XElement(RootElementName));
    }
}
