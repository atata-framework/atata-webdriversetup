namespace Atata.WebDriverSetup;

/// <summary>
/// Represents a driver target operating system platform information.
/// </summary>
public sealed class TargetOSPlatform
{
    private TargetOSPlatform(TargetOSFamily osFamily, TargetArchitecture architecture)
    {
        OSFamily = osFamily;
        Architecture = architecture;
    }

    /// <summary>
    /// Gets the architecture.
    /// </summary>
    public TargetArchitecture Architecture { get; }

    /// <summary>
    /// Gets the OS family.
    /// </summary>
    public TargetOSFamily OSFamily { get; }

    /// <summary>
    /// Gets the bits of OS.
    /// Returns either <c>64</c> or <c>32</c> value.
    /// </summary>
    public int Bits =>
        Architecture switch
        {
            TargetArchitecture.X32 => 32,
            TargetArchitecture.X64 or TargetArchitecture.Arm64 => 64,
            _ => throw new InvalidOperationException($"Unknown {nameof(TargetArchitecture)} value {Architecture}.")
        };

    /// <summary>
    /// Detects current platform automatically.
    /// </summary>
    /// <returns>The platform.</returns>
    public static TargetOSPlatform DetectAuto() =>
        Detect(WebDriverSetup.Architecture.Auto);

    /// <summary>
    /// Detects current platform automatically considering the specified <paramref name="architecture"/>.
    /// </summary>
    /// <param name="architecture">The target architecture.</param>
    /// <returns>The platform.</returns>
    public static TargetOSPlatform Detect(Architecture architecture) =>
        new(
            DetectOSFamily(),
            architecture == WebDriverSetup.Architecture.Auto ? DetectArchitecture() : MapArchitecture(architecture));

    private static TargetOSFamily DetectOSFamily() =>
        OSInfo.IsWindows
            ? TargetOSFamily.Windows
            : OSInfo.IsMacOS
                ? TargetOSFamily.MacOS
                : TargetOSFamily.Linux;

    private static TargetArchitecture DetectArchitecture() =>
        RuntimeInformation.OSArchitecture switch
        {
            RuntimeArchitecture.Arm64 => TargetArchitecture.Arm64,
            RuntimeArchitecture.X64 => TargetArchitecture.X64,
            _ => TargetArchitecture.X32
        };

    private static TargetArchitecture MapArchitecture(Architecture architecture) =>
        architecture switch
        {
            WebDriverSetup.Architecture.X32 => TargetArchitecture.X32,
            WebDriverSetup.Architecture.X64 => TargetArchitecture.X64,
            WebDriverSetup.Architecture.Arm64 => TargetArchitecture.Arm64,
            _ => throw new ArgumentException($"""Unsupported "{architecture}" architecture.""", nameof(architecture))
        };

    internal OSPlatforms ToOSPlatform() =>
        this switch
        {
            { OSFamily: TargetOSFamily.Windows, Architecture: TargetArchitecture.X32 } => OSPlatforms.Windows32,
            { OSFamily: TargetOSFamily.Windows, Architecture: TargetArchitecture.X64 } => OSPlatforms.Windows64,
            { OSFamily: TargetOSFamily.Windows, Architecture: TargetArchitecture.Arm64 } => OSPlatforms.WindowsArm64,
            { OSFamily: TargetOSFamily.MacOS, Architecture: TargetArchitecture.X64 } => OSPlatforms.Mac64,
            { OSFamily: TargetOSFamily.MacOS, Architecture: TargetArchitecture.Arm64 } => OSPlatforms.MacArm64,
            { OSFamily: TargetOSFamily.Linux } => OSPlatforms.Linux64,
            _ => throw new InvalidOperationException($"Cannot resolve {nameof(OSPlatforms)} value.")
        };
}
