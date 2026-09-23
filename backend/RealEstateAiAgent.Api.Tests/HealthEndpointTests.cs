using System.Net;
using System.Net.Http.Json;

namespace RealEstateAiAgent.Api.Tests;

[Collection("Api")]
public class HealthEndpointTests
{
    private readonly HttpClient _client;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsOk_AtUnversionedRoute()
    {
        var response = await _client.GetAsync("/api/Health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(body);
        Assert.Equal("ok", body.Status);
    }

    private sealed record HealthResponse(string Status, string Service, DateTime Utc);
}
