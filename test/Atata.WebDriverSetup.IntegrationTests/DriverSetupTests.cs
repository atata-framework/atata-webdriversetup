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
            if (Directory.Exists(DriverSetup.GlobalOptions.StorageDirectoryPath))
                Directory.Delete(DriverSetup.GlobalOptions.StorageDirectoryPath, true);
        }

        [TestCase(BrowserNames.Chrome)]
        [TestCase(BrowserNames.Firefox)]
        [TestCase(BrowserNames.Edge, ExcludePlatform = Platforms.Linux)]
        [TestCase(BrowserNames.Opera)]
        [TestCase(BrowserNames.InternetExplorer, IncludePlatform = Platforms.Windows)]
        public void WithAutoVersion(string browserName)
        {
            var result = DriverSetup.Configure(browserName)
                .WithAutoVersion()
                .SetUp();

            AssertDriverIsSetUp(result, browserName);
            AssertVersionCache(browserName);
        }

        [TestCase(BrowserNames.Chrome)]
        [TestCase(BrowserNames.Firefox)]
        [TestCase(BrowserNames.Edge, ExcludePlatform = Platforms.Linux)]
        [TestCase(BrowserNames.Opera)]
        [TestCase(BrowserNames.InternetExplorer, IncludePlatform = Platforms.Windows)]
        public void WithLatestVersion(string browserName)
        {
            var result = DriverSetup.Configure(browserName)
                .WithLatestVersion()
                .SetUp();

            AssertDriverIsSetUp(result, browserName);
            AssertVersionCache(browserName);
        }

        [TestCase(BrowserNames.Chrome, "87.0.4280.88")]
        [TestCase(BrowserNames.Firefox, "0.28.0")]
        [TestCase(BrowserNames.Edge, "89.0.774.4", ExcludePlatform = Platforms.Linux)]
        [TestCase(BrowserNames.Opera, "86.0.4240.80")]
        [TestCase(BrowserNames.InternetExplorer, "3.141.59", IncludePlatform = Platforms.Windows)]
        public void WithVersion(string browserName, string version)
        {
            var result = DriverSetup.Configure(browserName)
                .WithVersion(version)
                .SetUp();

            AssertDriverIsSetUp(result, browserName, version);
        }

        [TestCase(BrowserNames.Chrome, "87")]
        [TestCase(BrowserNames.Chrome, "87.0.4280")]
        [TestCase(BrowserNames.Edge, "89.0.774.4", ExcludePlatform = Platforms.Linux)]
        public void ByBrowserVersion(string browserName, string version)
        {
            var result = DriverSetup.Configure(browserName)
                .ByBrowserVersion(version)
                .SetUp();

            AssertDriverIsSetUp(result, browserName, version);
            AssertVersionCache(browserName, version);
        }

        [TestCase(BrowserNames.Firefox, "84")]
        [TestCase(BrowserNames.Opera, "73.0.3856.329")]
        [TestCase(BrowserNames.InternetExplorer, "11.0.0.4", IncludePlatform = Platforms.Windows)]
        public void ByBrowserVersion_Unsupported(string browserName, string version)
        {
            var builder = DriverSetup.Configure(browserName)
                .ByBrowserVersion(version);

            Assert.Throws<DriverSetupException>(() =>
                builder.SetUp());
        }

        private static void AssertDriverIsSetUp(
            DriverSetupResult setupResult,
            string browserName,
            string version = null)
        {
            var driverLocation = AssertDriverExists(browserName, version);

            AssertDriverSetupResult(setupResult, browserName, version, driverLocation);

            AssertPathEnvironmentVariable(driverLocation.DirectoryPath);
            AssertUniqueDriverEnvironmentVariable(browserName, driverLocation.DirectoryPath);
        }

        private static (string DirectoryPath, string FileName) AssertDriverExists(
            string browserName,
            string version)
        {
            string driverDirectoryPath = GetRootDriverDirectoryPath(browserName);

            Assert.That(driverDirectoryPath, Does.Exist);

            DirectoryInfo versionDirectory = new DirectoryInfo(driverDirectoryPath).GetDirectories()
                .Should().ContainSingle("there should exist single version folder")
                .Subject;

            if (version != null)
                versionDirectory.Name.Should().StartWith(version);

            versionDirectory.GetDirectories().Should().BeEmpty();

            FileInfo driverFile = versionDirectory.GetFiles().Should().ContainSingle()
                .Subject;

            return (versionDirectory.FullName, driverFile.Name);
        }

        private static string GetRootDriverDirectoryPath(string browserName) =>
            Path.Combine(
                DriverSetup.GlobalOptions.StorageDirectoryPath,
                browserName.Replace(" ", null).ToLower());

        private static void AssertDriverSetupResult(
            DriverSetupResult result,
            string browserName,
            string version,
            (string DirectoryPath, string FileName) driverLocation)
        {
            result.Should().NotBeNull();
            result.BrowserName.Should().Be(browserName);

            if (version != null)
                result.Version.Should().StartWith(version);

            result.DirectoryPath.Should().Be(driverLocation.DirectoryPath);
            result.FileName.Should().Be(driverLocation.FileName);
        }

        private static void AssertVersionCache(string browserName, string version = null)
        {
            string driverDirectoryPath = GetRootDriverDirectoryPath(browserName);
            string cacheFilePath = Path.Combine(driverDirectoryPath, "versioncache.xml");

            Assert.That(cacheFilePath, Does.Exist);

            if (version != null)
                File.ReadAllText(cacheFilePath).Should().Contain(version);
        }

        private static void AssertPathEnvironmentVariable(string driverDirectoryPath)
        {
            Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process)
                .Split(Path.PathSeparator)
                .Should().Contain(driverDirectoryPath);
        }

        private static void AssertUniqueDriverEnvironmentVariable(
            string browserName,
            string driverDirectoryPath)
        {
            string uniqueEnvironmentVariableName = $"{browserName.Replace(" ", null)}Driver";

            Environment.GetEnvironmentVariable(uniqueEnvironmentVariableName, EnvironmentVariableTarget.Process)
                .Should().Be(driverDirectoryPath);
        }
    }
}
