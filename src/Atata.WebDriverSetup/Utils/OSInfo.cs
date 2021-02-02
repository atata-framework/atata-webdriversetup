using System;
using System.Runtime.InteropServices;

namespace Atata.WebDriverSetup
{
    public static class OSInfo
    {
        public static bool IsWindows =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsLinux =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static bool IsOSX =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool Is64Bit =>
            Environment.Is64BitOperatingSystem;

        public static int Bits =>
            Environment.Is64BitOperatingSystem ? 64 : 32;
    }
}
