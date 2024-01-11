namespace Atata.WebDriverSetup.IntegrationTests;

public class DriverSetupTests : IntegrationTestFixture
{
    [TestCase(BrowserNames.Chrome)]
    public void AutoSetUp(string browserName)
    {
        var result = DriverSetup.AutoSetUp(browserName);

        AssertDriverIsSetUp(result, browserName);
        AssertVersionCache(browserName);
    }

    [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.Valid))]
    public void AutoSetUp(string[] browserNames)
    {
        var results = DriverSetup.AutoSetUp(browserNames);

        AssertAutoSetUpDriverResults(results, browserNames);
    }

    [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsNullValue))]
    public void AutoSetUp_ThrowsArgumentNullException(string[] browserNames) =>
        Assert.Throws<ArgumentNullException>(() =>
            DriverSetup.AutoSetUp(browserNames));

    [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsUnsupportedValue))]
    public void AutoSetUp_ThrowsArgumentException(string[] browserNames)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            DriverSetup.AutoSetUp(browserNames));

        exception.Message.Should().ContainEquivalentOf("unsupported");
    }

    [TestCase(BrowserNames.Chrome)]
    public async Task AutoSetUpAsync(string browserName)
    {
        var result = await DriverSetup.AutoSetUpAsync(browserName);

        AssertDriverIsSetUp(result, browserName);
        AssertVersionCache(browserName);
    }

    [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.Valid))]
    public async Task AutoSetUpAsync(string[] browserNames)
    {
        var results = await DriverSetup.AutoSetUpAsync(browserNames);

        AssertAutoSetUpDriverResults(results, browserNames);
    }

    [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsNullValue))]
    public void AutoSetUpAsync_ThrowsArgumentNullException(string[] browserNames) =>
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            DriverSetup.AutoSetUpAsync(browserNames));

    [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsUnsupportedValue))]
    public void AutoSetUpAsync_ThrowsArgumentException(string[] browserNames)
    {
        var exception = Assert.ThrowsAsync<ArgumentException>(() =>
            DriverSetup.AutoSetUpAsync(browserNames));

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

    [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.Valid))]
    [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsNullValue))]
    [TestCaseSource(typeof(BrowserNameSets), nameof(BrowserNameSets.ContainsUnsupportedValue))]
    public async Task AutoSetUpSafelyAsync(string[] browserNames)
    {
        var results = await DriverSetup.AutoSetUpSafelyAsync(browserNames);

        AssertAutoSetUpDriverResults(results, browserNames?.Where(IsValidBrowserName) ?? new string[0]);
    }

    [Test]
    public async Task SetUpAsync_DifferentBrowsers_InParallel()
    {
        var results = await Task.WhenAll(
            DriverSetup.ConfigureChrome().SetUpAsync(),
            DriverSetup.ConfigureFirefox().SetUpAsync(),
            DriverSetup.ConfigureEdge().SetUpAsync());

        AssertAutoSetUpDriverResults(results, new[] { BrowserNames.Chrome, BrowserNames.Firefox, BrowserNames.Edge });
    }

    [Test]
    public async Task SetUpAsync_SameBrowsers_WithMutex_InParallel()
    {
        var results = await Task.WhenAll(
            DriverSetup.ConfigureChrome().WithMutex(true).SetUpAsync(),
            DriverSetup.ConfigureChrome().WithMutex(true).SetUpAsync(),
            DriverSetup.ConfigureChrome().WithMutex(true).SetUpAsync());

        results.Should().HaveCount(3);
        results[0].Should().BeEquivalentTo(results[1]).And.BeEquivalentTo(results[2]);
        AssertDriverIsSetUp(results[0], BrowserNames.Chrome);
        AssertVersionCache(BrowserNames.Chrome);
    }

    private static void AssertAutoSetUpDriverResults(
        IEnumerable<DriverSetupResult> setupResults,
        IEnumerable<string> browserNames)
    {
        var distinctBrowserNames = browserNames.Distinct().ToArray();
        setupResults.Should().HaveCount(distinctBrowserNames.Length);

        foreach (string browserName in distinctBrowserNames)
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
        public static readonly IEnumerable<string[]> Valid =
        [
            [BrowserNames.Chrome],
            [BrowserNames.Chrome, BrowserNames.Chrome],
            [BrowserNames.Chrome, BrowserNames.Firefox]
        ];

        public static readonly IEnumerable<string[]> ContainsNullValue =
        [
            null,
            [null as string],
            [BrowserNames.Chrome, null]
        ];

        public static readonly IEnumerable<string[]> ContainsUnsupportedValue =
        [
            ["Unknown"],
            [BrowserNames.Chrome, "Unknown"]
        ];
    }
}
