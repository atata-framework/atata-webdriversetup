using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Atata.WebDriverSetup.IntegrationTests
{
    public class DriverSetupTests : IntegrationTestFixture
    {
        [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.Valid))]
        public void AutoSetUp(string[] browserNames)
        {
            var results = DriverSetup.AutoSetUp(browserNames);

            AssertAutoSetUpDriverResults(results, browserNames);
        }

        [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsNullValue))]
        public void AutoSetUp_ThrowsArgumentNullException(string[] browserNames)
        {
            Assert.Throws<ArgumentNullException>(() =>
                DriverSetup.AutoSetUp(browserNames));
        }

        [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsUnsupportedValue))]
        public void AutoSetUp_ThrowsArgumentException(string[] browserNames)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                DriverSetup.AutoSetUp(browserNames));

            exception.Message.Should().ContainEquivalentOf("unsupported");
        }

        [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.Valid))]
        [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsNullValue))]
        [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsUnsupportedValue))]
        public void AutoSetUpSafely(string[] browserNames)
        {
            var results = DriverSetup.AutoSetUpSafely(browserNames);

            AssertAutoSetUpDriverResults(results, browserNames?.Where(IsValidBrowserName) ?? new string[0]);
        }

        private static void AssertAutoSetUpDriverResults(
            IEnumerable<DriverSetupResult> setupResults,
            IEnumerable<string> browserNames)
        {
            setupResults.Should().HaveCount(browserNames.Count());

            foreach (string browserName in browserNames)
            {
                var correspondingResult = setupResults.Should()
                    .ContainSingle(x => x.BrowserName == browserName)
                    .Subject;

                AssertDriverIsSetUp(correspondingResult, browserName);
                AssertVersionCache(browserName);
            }
        }

        public static class BrowserNameSets
        {
            public static readonly IEnumerable<string[]> Valid = new[]
            {
                new[] { BrowserNames.Chrome },
                new[] { BrowserNames.Chrome, BrowserNames.Firefox }
            };

            public static readonly IEnumerable<string[]> ContainsNullValue = new string[][]
            {
                null,
                new[] { null as string },
                new[] { BrowserNames.Chrome, null }
            };

            public static readonly IEnumerable<string[]> ContainsUnsupportedValue = new string[][]
            {
                new[] { "Unknown" },
                new[] { BrowserNames.Chrome, "Unknown" }
            };
        }
    }
}
