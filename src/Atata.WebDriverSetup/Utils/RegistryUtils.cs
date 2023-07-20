using Microsoft.Win32;

namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a set of utility methods for working with a registry.
/// </summary>
public static class RegistryUtils
{
    private const string CurrentUserRegistryAppPathFormat =
        @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\{0}";

    private const string LocalMachineRegistryAppPathFormat =
        @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{0}";

    /// <summary>
    /// Gets the application path in registry.
    /// Tries to find the path in software paths of <c>HKEY_CURRENT_USER</c> and <c>HKEY_LOCAL_MACHINE</c>.
    /// </summary>
    /// <param name="applicationName">Name of the application.</param>
    /// <returns>The path or <see langword="null"/>.</returns>
    public static string GetApplicationPath(string applicationName) =>
        GetValue(string.Format(CurrentUserRegistryAppPathFormat, applicationName))
            ?? GetValue(string.Format(LocalMachineRegistryAppPathFormat, applicationName));

    /// <summary>
    /// Gets the registry value by key name and optionally by value name.
    /// </summary>
    /// <param name="keyName">Name of the key.</param>
    /// <param name="valueName">Name of the value.</param>
    /// <returns>The value or <see langword="null"/>.</returns>
    public static string GetValue(string keyName, string valueName = null)
    {
        try
        {
            return Registry.GetValue(keyName, valueName, null) as string;
        }
        catch
        {
            return null;
        }
    }
}
