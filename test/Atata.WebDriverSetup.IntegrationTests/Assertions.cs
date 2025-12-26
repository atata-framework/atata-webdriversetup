namespace Atata.WebDriverSetup.IntegrationTests;

internal static class Assertions
{
    internal static void AssertUrlReturnsOK(Uri url)
    {
        using HttpClient httpClient = new();
        using HttpRequestMessage requestMessage = new(HttpMethod.Head, url);
        using HttpResponseMessage response = SendHttpRequest(httpClient, requestMessage);

        response.StatusCode.Should().Be(HttpStatusCode.OK, $"URL {url} should be available");
    }

    private static HttpResponseMessage SendHttpRequest(HttpClient httpClient, HttpRequestMessage requestMessage) =>
#if NETFRAMEWORK
        httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
#else
        httpClient.Send(requestMessage);
#endif
}
