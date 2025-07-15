using Atata.Cli;

namespace Atata.WebDriverSetup;

/// <summary>
/// Provides a set of utility methods for the installed application version detection.
/// </summary>
public static class AppVersionDetector
{
    /// <summary>
    /// Gets the application version from "Program Files" and "Program Files (x86)" folders.
    /// </summary>
    /// <param name="applicationRelativePaths">The application relative paths in Program Files or Program Files (x86) folders.</param>
    /// <returns>The version or <see langword="null"/>.</returns>
    public static string? GetFromProgramFiles(params string[] applicationRelativePaths) =>
        GetFromProgramFiles(applicationRelativePaths.AsEnumerable());

    /// <summary>
    /// Gets the application version from "Program Files" and "Program Files (x86)" folders.
    /// </summary>
    /// <param name="applicationRelativePaths">The application relative paths in Program Files or Program Files (x86) folders.</param>
    /// <returns>The version or <see langword="null"/>.</returns>
    public static string? GetFromProgramFiles(IEnumerable<string> applicationRelativePaths)
    {
        string[] programFilesFolders =
        [
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        ];

        return programFilesFolders
            .Where(path => path?.Length > 0)
            .SelectMany(progPath => applicationRelativePaths.Select(relPath => Path.Combine(progPath, relPath)))
            .Select(GetFromExecutableFileVersion)
            .FirstOrDefault(x => x != null);
    }

    /// <summary>
    /// Gets the application version from executable file version information.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>The version or <see langword="null"/>.</returns>
    public static string? GetFromExecutableFileVersion(string filePath) =>
        File.Exists(filePath)
            ? GetFileVersion(filePath)
            : null;

    /// <summary>
    /// Gets the application version from "BLBeacon/version" key in registry.
    /// </summary>
    /// <param name="applicationRelativePathInSoftwareSection">The application relative path in software section.</param>
    /// <returns>The version or <see langword="null"/>.</returns>
    public static string? GetFromBLBeaconInRegistry(string applicationRelativePathInSoftwareSection) =>
        RegistryUtils.GetValue(
            $@"HKEY_CURRENT_USER\Software\{applicationRelativePathInSoftwareSection}\BLBeacon",
            "version");

    /// <summary>
    /// Gets the application version by application path in registry.
    /// </summary>
    /// <param name="applicationName">Name of the application.</param>
    /// <returns>The version or <see langword="null"/>.</returns>
    public static string? GetByApplicationPathInRegistry(string applicationName)
    {
        string? applicationPath = RegistryUtils.GetApplicationPath(applicationName);

        return applicationPath?.Length > 0
            ? GetFileVersion(applicationPath)
            : null;
    }

    private static string? GetFileVersion(string filePath)
    {
        try
        {
            return FileVersionInfo.GetVersionInfo(filePath).FileVersion;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the application version through OSX application CLI passing <c>"--version"</c> argument.
    /// </summary>
    /// <param name="applicationName">The application name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The version or <see langword="null"/>.</returns>
    public static async Task<string?> GetThroughOSXApplicationCliAsync(string applicationName, CancellationToken cancellationToken = default)
    {
        string filePath = $"/Applications/{applicationName}.app/Contents/MacOS/{applicationName}";

        return await GetThroughCliAsync(filePath, "--version", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the application version through CLI.
    /// </summary>
    /// <param name="fileNameOrCommand">The file name or command.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The version or <see langword="null"/>.</returns>
    public static async Task<string?> GetThroughCliAsync(string fileNameOrCommand, string arguments, CancellationToken cancellationToken = default)
    {
        try
        {
            ProgramCli cli = new(fileNameOrCommand);

            CliCommandResult cliResult = await cli.ExecuteAsync(arguments, cancellationToken)
                .ConfigureAwait(false);

            string result = cliResult.Output.Trim();

            Log.Trace($"Command \"{fileNameOrCommand} {arguments}\" => \"{result}\"");
            return result;
        }
        catch (Exception exception)
        {
            Log.Trace($"Command \"{fileNameOrCommand} {arguments}\" => \"{exception}\"");
            return null;
        }
    }
}
