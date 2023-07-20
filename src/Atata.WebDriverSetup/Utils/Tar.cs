namespace Atata.WebDriverSetup;

internal static class Tar
{
    internal static void ExtractTarGz(string filePath, string destinationDirectoryPath)
    {
        using var stream = File.OpenRead(filePath);

        ExtractTarGz(stream, destinationDirectoryPath);
    }

    internal static void ExtractTarGz(Stream stream, string destinationDirectoryPath)
    {
        using var gzip = new GZipStream(stream, CompressionMode.Decompress);
        using var memoryStream = new MemoryStream();

        gzip.CopyTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        ExtractTar(memoryStream, destinationDirectoryPath);
    }

    internal static void ExtractTar(string filePath, string destinationDirectoryPath)
    {
        using var stream = File.OpenRead(filePath);

        ExtractTar(stream, destinationDirectoryPath);
    }

    internal static void ExtractTar(Stream stream, string destinationDirectoryPath)
    {
        byte[] buffer = new byte[100];

        while (true)
        {
            stream.Read(buffer, 0, 100);
            string entryName = Encoding.ASCII.GetString(buffer).Trim('\0');

            if (string.IsNullOrWhiteSpace(entryName))
                return;

            stream.Seek(24, SeekOrigin.Current);
            stream.Read(buffer, 0, 12);
            long size = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

            stream.Seek(376L, SeekOrigin.Current);

            string destinationFilePath = Path.Combine(destinationDirectoryPath, entryName);

            if (!Directory.Exists(Path.GetDirectoryName(destinationFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));

            if (!entryName.Equals("./", StringComparison.Ordinal))
            {
                using FileStream fileStream = File.Open(destinationFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] fileBuffer = new byte[size];

                stream.Read(fileBuffer, 0, fileBuffer.Length);
                fileStream.Write(fileBuffer, 0, fileBuffer.Length);
            }

            long offset = 512 - (stream.Position % 512);
            if (offset == 512)
                offset = 0;

            stream.Seek(offset, SeekOrigin.Current);
        }
    }
}
