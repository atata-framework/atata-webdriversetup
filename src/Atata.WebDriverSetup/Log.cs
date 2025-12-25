#pragma warning disable IDE0022 // Use expression body for method

namespace Atata.WebDriverSetup;

internal static class Log
{
    internal static void Trace(string message)
    {
#if DEBUG
        Console.Out.WriteLine(message);
#endif
    }

    internal static void Warn(Exception exception, string message)
    {
#if DEBUG
        Warn($"{message} {exception}");
#endif
    }

    internal static void Warn(string message)
    {
#if DEBUG
        Console.Out.WriteLine(message);
#endif
    }
}
