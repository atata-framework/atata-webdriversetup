using System;
using System.IO;
using System.Linq;

namespace Atata.WebDriverSetup.IntegrationTests
{
    public static class PathSanitizer
    {
        private static readonly Lazy<char[]> s_invalidPathChars = new Lazy<char[]>(() => Path.GetInvalidFileNameChars());

        public static string SanitizeForFileName(string value) =>
            new string(value.Where(x => !s_invalidPathChars.Value.Contains(x)).ToArray());
    }
}
