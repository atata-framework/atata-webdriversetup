﻿using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public abstract class IntegrationTestFixture
    {
        private static readonly string[] ValidBrowserNames = new[]
        {
            BrowserNames.Chrome,
            BrowserNames.Firefox,
            BrowserNames.Edge,
            BrowserNames.Opera,
            BrowserNames.InternetExplorer
        };

        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists(DriverSetup.GlobalOptions.StorageDirectoryPath))
                Directory.Delete(DriverSetup.GlobalOptions.StorageDirectoryPath, true);
        }

        protected static void AssertDriverIsSetUp(
            DriverSetupResult setupResult,
            string browserName,
            string version = null,
            Architecture architecture = Architecture.Auto)
        {
            var driverLocation = AssertDriverExists(browserName, version, architecture);

            AssertDriverSetupResult(setupResult, browserName, version, driverLocation);

            AssertPathEnvironmentVariable(driverLocation.DirectoryPath);
            AssertUniqueDriverEnvironmentVariable(browserName, driverLocation.DirectoryPath);
        }

        private static (string DirectoryPath, string FileName) AssertDriverExists(
            string browserName,
            string version,
            Architecture architecture)
        {
            string driverDirectoryPath = GetRootDriverDirectoryPath(browserName);

            Assert.That(driverDirectoryPath, Does.Exist);

            DirectoryInfo versionDirectory = new DirectoryInfo(driverDirectoryPath).GetDirectories()
                .Should().ContainSingle("there should exist single version folder")
                .Subject;

            if (version != null)
                versionDirectory.Name.Should().StartWith(version);

            DirectoryInfo destinationDirectory;

            if (architecture == Architecture.Auto)
            {
                destinationDirectory = versionDirectory;
            }
            else
            {
                destinationDirectory = versionDirectory.GetDirectories()
                    .Should().ContainSingle("there should exist single architecture folder")
                    .Subject;

                destinationDirectory.Name.Should().Be(architecture.ToString().ToLower());
            }

            destinationDirectory.GetDirectories().Should().BeEmpty();

            FileInfo driverFile = destinationDirectory.GetFiles().Should().ContainSingle()
                .Subject;

            return (destinationDirectory.FullName, driverFile.Name);
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

        protected static void AssertVersionCache(string browserName, string version = null)
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

        protected static bool IsValidBrowserName(string browserName) =>
            ValidBrowserNames.Contains(browserName);
    }
}
