namespace Atata.WebDriverSetup.IntegrationTests;

public sealed class AsyncFileLockTests
{
    [Test]
    public async Task WaitAsync()
    {
        string lockFilePath = Path.Combine(Path.GetTempPath(), "test.lock");

        using AsyncFileLock fileLock = new(lockFilePath);
        bool isAcquired = await fileLock.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.That(isAcquired, Is.True);
        Assert.That(lockFilePath, Does.Exist);
    }

    [Test]
    public async Task WaitAsync_WhenLocked()
    {
        string lockFilePath = Path.Combine(Path.GetTempPath(), "test.lock");

        using AsyncFileLock fileLock1 = new(lockFilePath);
        await fileLock1.WaitAsync(TimeSpan.FromSeconds(5));

        using AsyncFileLock fileLock2 = new(lockFilePath);
        bool isSecondAcquired = await fileLock2.WaitAsync(TimeSpan.FromSeconds(2));

        Assert.That(isSecondAcquired, Is.False);
    }

    [Test]
    public async Task WaitAsync_WhenLockedAndUnlocked()
    {
        string lockFilePath = Path.Combine(Path.GetTempPath(), "test.lock");

        var task1 = Task.Run(async () =>
        {
            using AsyncFileLock fileLock1 = new(lockFilePath);
            await fileLock1.WaitAsync(TimeSpan.FromSeconds(5));
            await Task.Delay(500);
        });

        var task2 = Task.Run(async () =>
        {
            await Task.Delay(100);
            using AsyncFileLock fileLock2 = new(lockFilePath);
            return await fileLock2.WaitAsync(TimeSpan.FromSeconds(2));
        });

        bool isSecondAcquired = await task2;
        Assert.That(isSecondAcquired, Is.True);

        await task1;
    }
}
