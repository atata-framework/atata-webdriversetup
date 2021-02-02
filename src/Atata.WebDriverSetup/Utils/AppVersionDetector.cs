using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Atata.WebDriverSetup
{
    public static class AppVersionDetector
    {
        public static string GetFromProgramFiles(params string[] applicationRelativePaths) =>
            GetFromProgramFiles(applicationRelativePaths.AsEnumerable());

        public static string GetFromProgramFiles(IEnumerable<string> applicationRelativePaths)
        {
            string[] programFilesFolders = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };

            string applicationPath = programFilesFolders
                .Where(path => !string.IsNullOrEmpty(path))
                .SelectMany(progPath => applicationRelativePaths.Select(relPath => Path.Combine(progPath, relPath)))
                .FirstOrDefault(path => File.Exists(path));

            return applicationPath != null
                ? FileVersionInfo.GetVersionInfo(applicationPath).FileVersion
                : null;
        }

        public static string GetFromBLBeaconInRegistry(string applicationRelativePathInSoftwareSection) =>
            RegistryUtils.GetValue(
                $@"HKEY_CURRENT_USER\Software\{applicationRelativePathInSoftwareSection}\BLBeacon",
                "version");

        public static string GetByApplicationPathFromRegistry(string applicationName)
        {
            string applicationPath = RegistryUtils.GetApplicationPath(applicationName);

            return string.IsNullOrEmpty(applicationPath)
                ? null
                : FileVersionInfo.GetVersionInfo(applicationPath).FileVersion;
        }
    }
}
