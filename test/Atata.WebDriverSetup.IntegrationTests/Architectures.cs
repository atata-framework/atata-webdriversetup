namespace Atata.WebDriverSetup.IntegrationTests;

internal static class Architectures
{
    internal static IEnumerable<Architecture> All { get; } =
    [
        Architecture.X32,
        Architecture.X64,
        Architecture.Arm64
    ];
}
