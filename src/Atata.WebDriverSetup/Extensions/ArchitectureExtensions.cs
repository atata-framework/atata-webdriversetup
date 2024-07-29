namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a set of methods for <see cref="Architecture"/>.
/// </summary>
public static class ArchitectureExtensions
{
    /// <summary>
    /// Gets the bits of architecture.
    /// </summary>
    /// <param name="architecture">The architecture.</param>
    /// <returns>Either <c>64</c> or <c>32</c> value.</returns>
    public static int GetBits(this Architecture architecture) =>
        architecture switch
        {
            Architecture.X32 => 32,
            Architecture.X64 or Architecture.Arm64 => 64,
            _ => OSInfo.Bits
        };

    /// <summary>
    /// Resolves the concrete architecture.
    /// If it is <see cref="Architecture.Auto"/>, detects the actual current one.
    /// </summary>
    /// <param name="architecture">The architecture.</param>
    /// <returns>The architecture resolved.</returns>
    public static Architecture ResolveConcreteArchitecture(this Architecture architecture) =>
        architecture != Architecture.Auto
            ? architecture
            : DetectArchitecture();

    private static Architecture DetectArchitecture() =>
        RuntimeInformation.OSArchitecture switch
        {
            System.Runtime.InteropServices.Architecture.Arm64 => Architecture.Arm64,
            System.Runtime.InteropServices.Architecture.X64 => Architecture.X64,
            _ => Architecture.X32
        };
}
