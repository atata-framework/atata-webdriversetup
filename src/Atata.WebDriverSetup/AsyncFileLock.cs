namespace Atata.WebDriverSetup;

/// <summary>
/// Provides an asynchronous file-based locking mechanism to synchronize access across processes.
/// </summary>
/// <remarks>
/// This class implements a file-based locking mechanism that can be used to synchronize access
/// to shared resources across different processes. It uses a file on disk as a mutex.
/// </remarks>
public sealed class AsyncFileLock : IDisposable
{
    private readonly string _lockFilePath;

    private FileStream? _lockStream;

    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFileLock"/> class with the specified lock file path.
    /// </summary>
    /// <param name="lockFilePath">The path to the lock file.</param>
    public AsyncFileLock(string lockFilePath)
    {
        Guard.ThrowIfNullOrWhitespace(lockFilePath);
        _lockFilePath = lockFilePath;
    }

    /// <summary>
    /// Gets or sets the interval to wait between retry attempts when acquiring the lock.
    /// </summary>
    public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// Tries to acquire the file lock asynchronously within the given timeout.
    /// </summary>
    /// <param name="timeout">The maximum time to wait for acquiring the lock.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task with a boolean result indicating whether the lock was acquired successfully.
    /// </returns>
    public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(AsyncFileLock));

        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _lockStream = new FileStream(
                    _lockFilePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None,
                    bufferSize: 1,
                    useAsync: true);

                if (_lockStream.Length == 0)
                {
                    await _lockStream.WriteAsync([1], 0, 1, cancellationToken)
                        .ConfigureAwait(false);
                    await _lockStream.FlushAsync(cancellationToken)
                        .ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                await Task.Delay(RetryInterval, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        return false;
    }

    /// <summary>
    /// Releases the file lock and disposes the underlying resources.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        if (_lockStream is not null)
        {
            try
            {
                _lockStream.Dispose();
            }
            catch
            {
                // Ignore cleanup errors.
            }

            _lockStream = null;
        }

        _isDisposed = true;
    }
}
