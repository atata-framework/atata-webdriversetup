using Microsoft.Win32;

namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a set of utility methods for working with a registry.
/// </summary>
public static class RegistryUtils
{
    private const string CurrentUserRegistryAppPathPrefix =
        @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\";

    private const string LocalMachineRegistryAppPathPrefix =
        @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\";

    /// <summary>
    /// Gets the application path in registry.
    /// Tries to find the path in software paths of <c>HKEY_CURRENT_USER</c> and <c>HKEY_LOCAL_MACHINE</c>.
    /// </summary>
    /// <param name="applicationName">Name of the application.</param>
    /// <returns>The path or <see langword="null"/>.</returns>
    public static string? GetApplicationPath(string applicationName) =>
        GetValue(CurrentUserRegistryAppPathPrefix + applicationName)
            ?? GetValue(LocalMachineRegistryAppPathPrefix + applicationName);

    /// <summary>
    /// Gets the registry value by key name and optionally by value name.
    /// </summary>
    /// <param name="keyName">Name of the key.</param>
    /// <param name="valueName">Name of the value.</param>
    /// <returns>The value or <see langword="null"/>.</returns>
    public static string? GetValue(string keyName, string? valueName = null)
    {
#if NET8_0_OR_GREATER
        if (!OperatingSystem.IsWindows())
            return null;
#endif

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
