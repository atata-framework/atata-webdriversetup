using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class DriverSetupTests
    {
        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists(DriverSetup.GlobalOptions.CachePath))
                Directory.Delete(DriverSetup.GlobalOptions.CachePath, true);
        }

        [TestCase(BrowserNames.Chrome)]
        [TestCase(BrowserNames.Firefox)]
        [TestCase(BrowserNames.Edge, Category = TestCategories.UnsupportedOnLinux)]
        [TestCase(BrowserNames.Opera)]
        [TestCase(BrowserNames.InternetExplorer)]
        public void WithAutoVersion(string browserName)
        {
            DriverSetup.Configure(browserName)
                .WithAutoVersion()
                .SetUp();

            AssertDriverAndEnvironmentVariablesExists(browserName);
        }

        [TestCase(BrowserNames.Chrome)]
        [TestCase(BrowserNames.Firefox)]
        [TestCase(BrowserNames.Edge, Category = TestCategories.UnsupportedOnLinux)]
        [TestCase(BrowserNames.Opera)]
        [TestCase(BrowserNames.InternetExplorer)]
        public void WithLatestVersion(string browserName)
        {
            DriverSetup.Configure(browserName)
                .WithLatestVersion()
                .SetUp();

            AssertDriverAndEnvironmentVariablesExists(browserName);
        }

        [TestCase(BrowserNames.Chrome, "87.0.4280.88")]
        [TestCase(BrowserNames.Firefox, "0.28.0")]
        [TestCase(BrowserNames.Edge, "89.0.774.4", Category = TestCategories.UnsupportedOnLinux)]
        [TestCase(BrowserNames.Opera, "86.0.4240.80")]
        [TestCase(BrowserNames.InternetExplorer, "3.141.59")]
        public void WithVersion(string browserName, string version)
        {
            DriverSetup.Configure(browserName)
                .WithVersion(version)
                .SetUp();

            AssertDriverAndEnvironmentVariablesExists(browserName, version);
        }

        [TestCase(BrowserNames.Chrome, "87")]
        [TestCase(BrowserNames.Chrome, "87.0.4280")]
        [TestCase(BrowserNames.Edge, "89.0.774.4", Category = TestCategories.UnsupportedOnLinux)]
        public void ByBrowserVersion(string browserName, string version)
        {
            DriverSetup.Configure(browserName)
                .ByBrowserVersion(version)
                .SetUp();

            AssertDriverAndEnvironmentVariablesExists(browserName, version);
        }

        [TestCase(BrowserNames.Firefox, "84")]
        [TestCase(BrowserNames.Opera, "73.0.3856.329")]
        [TestCase(BrowserNames.InternetExplorer, "11.0.0.4")]
        public void ByBrowserVersion_Unsupported(string browserName, string version)
        {
            var builder = DriverSetup.Configure(browserName)
                .ByBrowserVersion(version);

            Assert.Throws<DriverSetupException>(() =>
                builder.SetUp());
        }

        private static void AssertDriverAndEnvironmentVariablesExists(string browserName, string version = null)
        {
            string driverDirectoryPath = AssertDriverExists(browserName, version);

            AssertPathEnvironmentVariable(driverDirectoryPath);
            AssertUniqueDriverEnvironmentVariable(browserName, driverDirectoryPath);
        }

        private static string AssertDriverExists(string browserName, string version = null)
        {
            string driverDirectoryPath = Path.Combine(
                DriverSetup.GlobalOptions.CachePath,
                browserName.Replace(" ", null).ToLower());

            Assert.That(driverDirectoryPath, Does.Exist);

            DirectoryInfo versionDirectory = new DirectoryInfo(driverDirectoryPath).GetDirectories()
                .Should().ContainSingle("there should exist single version folder")
                .Subject;

            if (version != null)
                versionDirectory.Name.Should().StartWith(version);

            versionDirectory.GetDirectories().Should().BeEmpty();
            versionDirectory.GetFiles().Should().ContainSingle();

            return versionDirectory.FullName;
        }

        private static void AssertPathEnvironmentVariable(string driverDirectoryPath)
        {
            Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process)
                .Split(Path.PathSeparator)
                .Should().Contain(driverDirectoryPath);
        }

        private static void AssertUniqueDriverEnvironmentVariable(string browserName, string driverDirectoryPath)
        {
            string uniqueEnvironmentVariableName = $"Atata.{browserName.Replace(" ", null)}Driver";

            Environment.GetEnvironmentVariable(uniqueEnvironmentVariableName, EnvironmentVariableTarget.Process)
                .Should().Be(driverDirectoryPath);
        }
    }
}
