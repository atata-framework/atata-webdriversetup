namespace Atata.WebDriverSetup;

internal static class Log
{
    internal static void Trace(string message)
    {
#if DEBUG
#pragma warning disable IDE0022 // Use expression body for method
        Console.Out.WriteLine(message);
#pragma warning restore IDE0022 // Use expression body for method
#endif
    }

    internal static void Warn(Exception exception, string message)
    {
#if DEBUG
#pragma warning disable IDE0022 // Use expression body for method
        Warn($"{message} {exception}");
#pragma warning restore IDE0022 // Use expression body for method
#endif
    }

    internal static void Warn(string message)
    {
#if DEBUG
#pragma warning disable IDE0022 // Use expression body for method
        Console.Out.WriteLine(message);
#pragma warning restore IDE0022 // Use expression body for method
#endif
    }
}
