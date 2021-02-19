using System;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the strategy of driver setup.
    /// </summary>
    public interface IDriverSetupStrategy
    {
        /// <summary>
        /// Gets the name of the driver binary file.
        /// </summary>
        string DriverBinaryFileName { get; }

        /// <summary>
        /// Gets the driver download URL.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="architecture">The architecture.</param>
        /// <returns>The driver download URL.</returns>
        Uri GetDriverDownloadUrl(string version, Architecture architecture);
    }
}
