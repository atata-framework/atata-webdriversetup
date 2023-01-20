using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;

namespace Atata.WebDriverSetup.IntegrationTests;

internal static class Assertions
{
    internal static void AssertUrlReturnsOK(Uri url)
    {
        using var httpClient = new HttpClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Head, url);
        using var response = httpClient.Send(requestMessage);

        response.StatusCode.Should().Be(HttpStatusCode.OK, $"URL {url} should be available");
    }
}
