namespace Atata.WebDriverSetup.IntegrationTests;

[SetUpFixture]
public sealed class GlobalFixture
{
    [OneTimeSetUp]
    public void GlobalSetUp()
    {
#if !NETFRAMEWORK
        if (OSInfo.IsMacOS)
            DriverSetup.GlobalConfiguration.WithCheckCertificateRevocationList(false);
#endif
    }
}
