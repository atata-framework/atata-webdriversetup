using System;
using System.Runtime.InteropServices;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Provides a set of operating system specific informational properties.
    /// </summary>
    public static class OSInfo
    {
        /// <summary>
        /// Gets a value indicating whether the current OS is Windows.
        /// </summary>
        public static bool IsWindows =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Gets a value indicating whether the current OS is Linux.
        /// </summary>
        public static bool IsLinux =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        /// Gets a value indicating whether the current OS is macOS.
        /// </summary>
        public static bool IsOSX =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// Gets a value indicating whether the current OS is 64-bit.
        /// </summary>
        public static bool Is64Bit =>
            Environment.Is64BitOperatingSystem;

        /// <summary>
        /// Gets the bits of OS.
        /// Returns either <c>64</c> or <c>32</c> value.
        /// </summary>
        public static int Bits =>
            Environment.Is64BitOperatingSystem ? 64 : 32;
    }
}
