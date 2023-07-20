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
        architecture == Architecture.X32
            ? 32
            : architecture is Architecture.X64 or Architecture.Arm64
                ? 64
                : OSInfo.Bits;
}
