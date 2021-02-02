using Microsoft.Win32;

namespace Atata.WebDriverSetup
{
    public static class RegistryUtils
    {
        private const string CurrentUserRegistryAppPathFormat =
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\{0}";

        private const string LocalMachineRegistryAppPathFormat =
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{0}";

        public static string GetApplicationPath(string applicationName)
        {
            return GetValue(string.Format(CurrentUserRegistryAppPathFormat, applicationName))
                ?? GetValue(string.Format(LocalMachineRegistryAppPathFormat, applicationName));
        }

        public static string GetValue(string keyName, string valueName = null)
        {
            return Registry.GetValue(keyName, valueName, null) as string;
        }
    }
}
