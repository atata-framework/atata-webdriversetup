using System.Collections.Generic;

namespace Atata.WebDriverSetup.IntegrationTests;

internal static class Architectures
{
    internal static IEnumerable<Architecture> All { get; } = new[]
    {
        Architecture.X32,
        Architecture.X64,
        Architecture.Arm64
    };
}
