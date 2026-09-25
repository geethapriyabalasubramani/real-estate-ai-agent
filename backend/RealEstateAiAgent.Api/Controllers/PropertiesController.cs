using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Data;
using RealEstateAiAgent.Api.Services;

namespace RealEstateAiAgent.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly IPropertySearchService _propertySearch;

    public PropertiesController(
        ApplicationDbContext db,
        IMemoryCache cache,
        IPropertySearchService propertySearch)
    {
        _db = db;
        _cache = cache;
        _propertySearch = propertySearch;
    }

    [HttpGet]
    public async Task<ActionResult<PagedPropertyResponse>> Search(
        [FromQuery] PropertySearchQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var cacheKey = BuildSearchCacheKey(query);
        if (_cache.TryGetValue(cacheKey, out PagedPropertyResponse? cachedResponse)
            && cachedResponse is not null)
        {
            Response.Headers["X-Cache"] = "HIT";
            return Ok(cachedResponse);
        }

        Response.Headers["X-Cache"] = "MISS";

        var response = await _propertySearch.SearchAsync(query, cancellationToken);

        _cache.Set(
            cacheKey,
            response,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
            });

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PropertyDetailDto>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var property = await _db.Properties
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PropertyDetailDto(
                p.Id,
                p.AddressLine1,
                p.City,
                p.State,
                p.PostalCode,
                p.Bedrooms,
                p.Bathrooms,
                p.Price,
                p.SquareFeet,
                p.HasGarage,
                p.Description,
                p.CreatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (property is null)
        {
            return NotFound(new { error = $"Property '{id}' was not found." });
        }

        return Ok(property);
    }

    private static string BuildSearchCacheKey(PropertySearchQuery query)
    {
        static string N(string? value) => string.IsNullOrWhiteSpace(value) ? "-" : value.Trim().ToLowerInvariant();

        return string.Join('|', new[]
        {
            "properties-search-v1",
            N(query.City),
            query.Bedrooms?.ToString() ?? "-",
            query.MinPrice?.ToString() ?? "-",
            query.MaxPrice?.ToString() ?? "-",
            query.HasGarage?.ToString() ?? "-",
            query.Page.ToString(),
            query.PageSize.ToString(),
        });
    }
}
