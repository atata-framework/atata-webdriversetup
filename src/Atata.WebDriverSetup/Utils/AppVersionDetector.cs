using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Atata.Cli;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a set of utility methods for the installed application version detection.
    /// </summary>
    public static class AppVersionDetector
    {
        /// <summary>
        /// Gets the application version from "Program Files" and "Program Files (x86)" folders.
        /// </summary>
        /// <param name="applicationRelativePaths">The application relative paths in Program Files or Program Files (x86) folders.</param>
        /// <returns>The version or <see langword="null"/>.</returns>
        public static string GetFromProgramFiles(params string[] applicationRelativePaths) =>
            GetFromProgramFiles(applicationRelativePaths.AsEnumerable());

        /// <summary>
        /// Gets the application version from "Program Files" and "Program Files (x86)" folders.
        /// </summary>
        /// <param name="applicationRelativePaths">The application relative paths in Program Files or Program Files (x86) folders.</param>
        /// <returns>The version or <see langword="null"/>.</returns>
        public static string GetFromProgramFiles(IEnumerable<string> applicationRelativePaths)
        {
            string[] programFilesFolders = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };

            return programFilesFolders
                .Where(path => !string.IsNullOrEmpty(path))
                .SelectMany(progPath => applicationRelativePaths.Select(relPath => Path.Combine(progPath, relPath)))
                .Select(GetFromExecutableFileVersion)
                .FirstOrDefault(x => x != null);
        }

        /// <summary>
        /// Gets the application version from executable file version information.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The version or <see langword="null"/>.</returns>
        public static string GetFromExecutableFileVersion(string filePath) =>
            File.Exists(filePath)
                ? FileVersionInfo.GetVersionInfo(filePath).FileVersion
                : null;

        /// <summary>
        /// Gets the application version from "BLBeacon/version" key in registry.
        /// </summary>
        /// <param name="applicationRelativePathInSoftwareSection">The application relative path in software section.</param>
        /// <returns>The version or <see langword="null"/>.</returns>
        public static string GetFromBLBeaconInRegistry(string applicationRelativePathInSoftwareSection) =>
            RegistryUtils.GetValue(
                $@"HKEY_CURRENT_USER\Software\{applicationRelativePathInSoftwareSection}\BLBeacon",
                "version");

        /// <summary>
        /// Gets the application version from application key in registry.
        /// </summary>
        /// <param name="applicationRelativePathInSoftwareSection">The application relative path in software section.</param>
        /// <returns>The version or <see langword="null"/>.</returns>
        public static string GetFromApplicationKeyInRegistry(string applicationRelativePathInSoftwareSection) =>
            RegistryUtils.GetValue(
                $@"HKEY_LOCAL_MACHINE\Software\{applicationRelativePathInSoftwareSection}");

        /// <summary>
        /// Gets the application version by application path in registry.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns>The version or <see langword="null"/>.</returns>
        public static string GetByApplicationPathInRegistry(string applicationName)
        {
            string applicationPath = RegistryUtils.GetApplicationPath(applicationName);

            return string.IsNullOrEmpty(applicationPath)
                ? null
                : FileVersionInfo.GetVersionInfo(applicationPath).FileVersion;
        }

        /// <summary>
        /// Gets the application version through OSX application CLI passing <c>"--version"</c> argument.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <returns>The version or <see langword="null"/>.</returns>
        public static string GetThroughOSXApplicationCli(string applicationName)
        {
            string filePath = $"/Applications/{applicationName}.app/Contents/MacOS/{applicationName}";

            string versionString = GetThroughCli(filePath, "--version");

            return versionString.Replace($"{applicationName} ", null);
        }

        /// <summary>
        /// Gets the application version through CLI.
        /// </summary>
        /// <param name="fileNameOrCommand">The file name or command.</param>
        /// <param name="arguments">The command arguments.</param>
        /// <param name="postprocess">Output postprocessing code if needed.</param>
        /// <returns>The version or <see langword="null"/>.</returns>
        public static string GetThroughCli(string fileNameOrCommand, string arguments, Func<string, string> postprocess = null)
        {
            try
            {
                var output = new ProgramCli(fileNameOrCommand).Execute(arguments).Output;
                output = postprocess?.Invoke(output);
                return output;
            }
            catch
            {
                return null;
            }
        }
    }
}
