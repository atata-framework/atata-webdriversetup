using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Atata.WebDriverSetup.IntegrationTests;

public class SeleniumManagerTests
{
    [Test]
    public void Edge()
    {
        var options = new EdgeOptions();

        string driverPath = new DriverFinder(options).GetDriverPath();
        Console.WriteLine(driverPath);

        driverPath.Should().NotBeNullOrEmpty();
    }
}
