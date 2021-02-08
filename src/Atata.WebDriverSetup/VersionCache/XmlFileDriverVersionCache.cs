using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Atata.WebDriverSetup
{
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

        private readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFileDriverVersionCache"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public XmlFileDriverVersionCache(string filePath)
        {
            this.filePath = filePath;
        }

        /// <inheritdoc/>
        public string GetOrAddLatest(DateTime minimumAcceptableTimestamp, Func<string> latestVersionResolveFunction)
        {
            latestVersionResolveFunction.CheckNotNull(nameof(latestVersionResolveFunction));

            return GetOrAdd(DriverVersions.Latest, minimumAcceptableTimestamp, v => latestVersionResolveFunction());
        }

        /// <inheritdoc/>
        public string GetOrAdd(string browserVersion, DateTime minimumAcceptableTimestamp, Func<string, string> versionResolveFunction)
        {
            browserVersion.CheckNotNullOrWhitespace(nameof(browserVersion));
            versionResolveFunction.CheckNotNull(nameof(versionResolveFunction));

            XDocument document = OpenDocument();

            XElement item = document.XPathSelectElement(
                $"//{RootElementName}/{ItemElementName}[@{BrowserAttributeName}='{browserVersion}']");

            string driverVersion;

            if (item != null)
            {
                if (TryGetDriverVersion(item, minimumAcceptableTimestamp, out driverVersion))
                    return driverVersion;
            }
            else
            {
                item = new XElement(ItemElementName, new XAttribute(BrowserAttributeName, browserVersion));
                document.Root.Add(item);
            }

            driverVersion = versionResolveFunction.Invoke(browserVersion);

            if (!string.IsNullOrWhiteSpace(driverVersion))
            {
                item.SetAttributeValue(DriverAttributeName, driverVersion);
                item.SetAttributeValue(TimestampAttributeName, DateTime.UtcNow.ToString(TimestampFormat, CultureInfo.InvariantCulture));

                document.Save(filePath);
                return driverVersion;
            }
            else
            {
                return null;
            }
        }

        private static bool TryGetDriverVersion(XElement item, DateTime minimumAcceptableTimestamp, out string version)
        {
            string timestampAsString = item.Attribute(TimestampAttributeName)?.Value;

            string driverVersion = item.Attribute(DriverAttributeName)?.Value;

            if (!string.IsNullOrEmpty(timestampAsString) && !string.IsNullOrEmpty(driverVersion))
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
            if (File.Exists(filePath))
            {
                XDocument document = XDocument.Load(filePath);

                if (document.Root.Name == RootElementName)
                    return document;
            }
            else
            {
                string directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }

            return new XDocument(
                new XElement(RootElementName));
        }
    }
}
