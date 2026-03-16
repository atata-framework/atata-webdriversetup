namespace Atata.WebDriverSetup.IntegrationTests;

[SetUpFixture]
public sealed class GlobalFixture
{
    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        if (OSInfo.IsOSX)
            DriverSetup.GlobalConfiguration.WithCheckCertificateRevocationList(false);
    }
}
