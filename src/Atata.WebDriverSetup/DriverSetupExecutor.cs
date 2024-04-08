namespace Atata.WebDriverSetup;

internal sealed class DriverSetupExecutor
{
    private readonly string _browserName;

    private readonly DriverSetupConfiguration _configuration;

    private readonly IDriverSetupStrategy _setupStrategy;

    private readonly IHttpRequestExecutor _httpRequestExecutor;

    internal DriverSetupExecutor(
        string browserName,
        DriverSetupConfiguration configuration,
        IDriverSetupStrategy setupStrategy,
        IHttpRequestExecutor httpRequestExecutor)
    {
        _browserName = browserName;
        _configuration = configuration;
        _setupStrategy = setupStrategy;
        _httpRequestExecutor = httpRequestExecutor;
    }

    internal DriverSetupResult SetUp(string version)
    {
        version.CheckNotNullOrWhitespace(nameof(version));

        string driverDestinationDirectoryPath = BuildDriverDestinationDirectoryPath(version);
        string driverDestinationFileName = _setupStrategy.DriverBinaryFileName;

        DownloadDriverIfMissing(version, driverDestinationDirectoryPath, driverDestinationFileName);

        SetEnvironmentVariables(driverDestinationDirectoryPath);

        return new DriverSetupResult(
            _browserName,
            version,
            driverDestinationDirectoryPath,
            driverDestinationFileName);
    }

    private void DownloadDriverIfMissing(
        string version,
        string driverDestinationDirectoryPath,
        string driverDestinationFileName)
    {
        string driverDestinationFilePath = Path.Combine(driverDestinationDirectoryPath, driverDestinationFileName);

        if (!File.Exists(driverDestinationFilePath))
        {
            if (!Directory.Exists(driverDestinationDirectoryPath))
                Directory.CreateDirectory(driverDestinationDirectoryPath);

            string downloadDestinationDirectoryPath = Path.Combine(driverDestinationDirectoryPath, "dl");

            if (!Directory.Exists(downloadDestinationDirectoryPath))
                Directory.CreateDirectory(downloadDestinationDirectoryPath);

            Architecture architecture = ResolveArchitecture();

            string downloadUrl = _setupStrategy.GetDriverDownloadUrl(version, architecture).AbsoluteUri;
            string downloadFileName = Path.GetFileName(downloadUrl);

            string downloadFilePath = Path.Combine(downloadDestinationDirectoryPath, downloadFileName);
            _httpRequestExecutor.DownloadFile(downloadUrl, downloadFilePath);

            try
            {
                ExtractDownloadedFile(downloadFilePath, driverDestinationFilePath);
                GrantFileExecutePermission(driverDestinationFilePath);
            }
            finally
            {
                Directory.Delete(downloadDestinationDirectoryPath, true);
            }
        }
    }

    private Architecture ResolveArchitecture() =>
        _configuration.Architecture != Architecture.Auto
            ? _configuration.Architecture
            : DetectArchitecture();

    private static Architecture DetectArchitecture() =>
        RuntimeInformation.OSArchitecture switch
        {
            System.Runtime.InteropServices.Architecture.Arm64 => Architecture.Arm64,
            System.Runtime.InteropServices.Architecture.X64 => Architecture.X64,
            _ => Architecture.X32
        };

    private string BuildDriverDestinationDirectoryPath(string version)
    {
        string path = Path.Combine(
            _configuration.StorageDirectoryPath,
            _browserName.Replace(" ", null).ToLower(CultureInfo.InvariantCulture),
            version);

        return _configuration.Architecture == Architecture.Auto
            ? path
            : Path.Combine(path, _configuration.Architecture.ToString().ToLower(CultureInfo.InvariantCulture));
    }

    private static void ExtractDownloadedFile(string sourceFilePath, string destinationFilePath)
    {
        if (sourceFilePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            File.Copy(sourceFilePath, destinationFilePath);
        }
        else if (sourceFilePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            ExtractFromZip(sourceFilePath, destinationFilePath);
        }
        else if (sourceFilePath.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
        {
            ExtractFromTarGz(sourceFilePath, destinationFilePath);
        }
    }

    private static void GrantFileExecutePermission(string filePath)
    {
        if (!OSInfo.IsWindows)
        {
            string chmodArguments = $"+x {filePath}";
            bool isChmodExecuted = Process.Start("chmod", chmodArguments)
                .WaitForExit(15000);

            if (!isChmodExecuted)
                throw new TimeoutException($"""Timed out waiting for "chmod {chmodArguments}" command.""");
        }
    }

    private static void ExtractFromZip(string archiveFilePath, string destinationFilePath)
    {
        string destinationFileName = Path.GetFileName(destinationFilePath);

        using var zip = ZipFile.OpenRead(archiveFilePath);
        var foundEntry = zip.Entries.FirstOrDefault(x => x.Name == destinationFileName);

        if (foundEntry != null)
            foundEntry.ExtractToFile(destinationFilePath, true);
        else
            throw new FileNotFoundException($"""Failed to find "{destinationFileName}" file in "{archiveFilePath}" archive.""");
    }

    private static void ExtractFromTarGz(string archiveFilePath, string destinationFilePath)
    {
        Tar.ExtractTarGz(archiveFilePath, Path.GetDirectoryName(destinationFilePath));

        if (!File.Exists(destinationFilePath))
            throw new FileNotFoundException("Failed to find file after extraction.", destinationFilePath);
    }

    private void SetEnvironmentVariables(string driverDirectoryPath)
    {
        if (_configuration.AddToEnvironmentPathVariable)
            AddToEnvironmentPathVariable(driverDirectoryPath);

        if (!string.IsNullOrWhiteSpace(_configuration.EnvironmentVariableName))
            Environment.SetEnvironmentVariable(_configuration.EnvironmentVariableName, driverDirectoryPath, EnvironmentVariableTarget.Process);
    }

    private static void AddToEnvironmentPathVariable(string path)
    {
        const string pathVariableName = "PATH";
        string pathVariableValue = Environment.GetEnvironmentVariable(pathVariableName, EnvironmentVariableTarget.Process);

        Regex isTherePathRegex = new Regex($"(^|{Path.PathSeparator}){Regex.Escape(path)}({Path.PathSeparator}|$)");

        if (!isTherePathRegex.IsMatch(pathVariableValue))
        {
            var newPathVariableValue = string.IsNullOrEmpty(pathVariableValue)
                ? path
                : $"{path}{Path.PathSeparator}{pathVariableValue}";

            Environment.SetEnvironmentVariable(pathVariableName, newPathVariableValue, EnvironmentVariableTarget.Process);
        }
    }
}
