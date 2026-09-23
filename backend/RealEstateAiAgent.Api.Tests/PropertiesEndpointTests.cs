using System.Net;
using System.Net.Http.Json;
using RealEstateAiAgent.Api.Contracts;

namespace RealEstateAiAgent.Api.Tests;

[Collection("Api")]
public class PropertiesEndpointTests
{
    private readonly HttpClient _client;

    public PropertiesEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    [Fact]
    public async Task Search_ReturnsOk_AndSeededItems()
    {
        var response = await _client.GetAsync("/api/v1/properties?page=1&pageSize=50");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PagedPropertyResponse>();
        Assert.NotNull(body);
        Assert.Equal(10, body.TotalCount);
        Assert.Equal(10, body.Items.Count);
    }

    [Fact]
    public async Task Search_FiltersByBedrooms_WithoutCity()
    {
        var response = await _client.GetAsync("/api/v1/properties?bedrooms=2&page=1&pageSize=50");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<PagedPropertyResponse>();

        Assert.NotNull(body);
        Assert.True(body.Items.Count > 0);
        Assert.All(body.Items, item => Assert.Equal(2, item.Bedrooms));
    }

    [Fact]
    public async Task Search_Returns400_WhenPageIsInvalid()
    {
        var response = await _client.GetAsync("/api/v1/properties?page=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_Returns400_WhenMinPriceGreaterThanMaxPrice()
    {
        var response = await _client.GetAsync("/api/v1/properties?minPrice=500000&maxPrice=100000");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_Returns200_ForSeededProperty()
    {
        var id = Guid.Parse("11111111-1111-1111-1111-111111111101");
        var response = await _client.GetAsync($"/api/v1/properties/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PropertyDetailDto>();
        Assert.NotNull(body);
        Assert.Equal("San Jose", body.City);
        Assert.False(string.IsNullOrWhiteSpace(body.Description));
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var id = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var response = await _client.GetAsync($"/api/v1/properties/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}