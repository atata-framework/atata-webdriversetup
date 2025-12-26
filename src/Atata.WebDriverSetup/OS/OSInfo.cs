namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a set of operating system specific informational properties.
/// </summary>
public static class OSInfo
{
    /// <summary>
    /// Gets a value indicating whether the current OS is Windows.
    /// </summary>
    public static bool IsWindows =>
#if NET8_0_OR_GREATER
        OperatingSystem.IsWindows();
#else
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif

    /// <summary>
    /// Gets a value indicating whether the current OS is Linux.
    /// </summary>
    public static bool IsLinux =>
#if NET8_0_OR_GREATER
        OperatingSystem.IsLinux();
#else
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#endif

    /// <summary>
    /// Gets a value indicating whether the current OS is macOS.
    /// </summary>
    public static bool IsMacOS =>
#if NET8_0_OR_GREATER
        OperatingSystem.IsMacOS();
#else
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif

    /// <summary>
    /// Gets a value indicating whether the current OS is 64-bit.
    /// </summary>
    public static bool Is64Bit =>
        RuntimeInformation.OSArchitecture is RuntimeArchitecture.Arm64 or RuntimeArchitecture.X64;

    /// <summary>
    /// Gets the bits of OS.
    /// Returns either <c>64</c> or <c>32</c> value.
    /// </summary>
    public static int Bits =>
        Is64Bit ? 64 : 32;
}
