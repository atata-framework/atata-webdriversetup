namespace Atata.WebDriverSetup.IntegrationTests;

[SetUpFixture]
public sealed class GlobalFixture
{
    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        if (OperatingSystem.IsMacOS())
            DriverSetup.GlobalConfiguration.WithCheckCertificateRevocationList(false);
    }
}
