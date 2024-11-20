namespace Atata.WebDriverSetup;

[Flags]
internal enum OSPlatforms
{
    None = 0,

    Windows32 = 0x0000_0001,

    Windows64 = 0x0000_0010,

    WindowsArm64 = 0x0000_0100,

    Mac64 = 0x0000_1000,

    MacArm64 = 0x0001_0000,

    Linux64 = 0x0010_0000
}
