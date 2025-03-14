namespace Atata.WebDriverSetup.IntegrationTests;

public static class PathSanitizer
{
    private static readonly Lazy<char[]> s_invalidPathChars = new(Path.GetInvalidFileNameChars);

    public static string SanitizeForFileName(string value) =>
        new([.. value.Where(x => !s_invalidPathChars.Value.Contains(x))]);
}
