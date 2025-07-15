﻿namespace Atata.WebDriverSetup.IntegrationTests;

internal static class Assertions
{
    internal static void AssertUrlReturnsOK(Uri url)
    {
        using HttpClient httpClient = new();
        using HttpRequestMessage requestMessage = new(HttpMethod.Head, url);
        using HttpResponseMessage response = httpClient.Send(requestMessage);

        response.StatusCode.Should().Be(HttpStatusCode.OK, $"URL {url} should be available");
    }
}
