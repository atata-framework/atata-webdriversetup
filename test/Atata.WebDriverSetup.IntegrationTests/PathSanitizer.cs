using System;
using System.IO;
using System.Linq;

namespace Atata.WebDriverSetup.IntegrationTests
{
    public static class PathSanitizer
    {
        private static readonly Lazy<char[]> InvalidPathChars = new Lazy<char[]>(() => Path.GetInvalidFileNameChars());

        public static string SanitizeForFileName(string value) =>
            new string(value.Where(x => !InvalidPathChars.Value.Contains(x)).ToArray());
    }
}
