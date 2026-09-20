using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Data;

namespace RealEstateAiAgent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PropertiesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<PagedPropertyResponse>> Search(
        [FromQuery] string? city,
        [FromQuery] int? bedrooms,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? hasGarage,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            return BadRequest(new { error = "page must be >= 1." });
        }

        if (pageSize is < 1 or > 50)
        {
            return BadRequest(new { error = "pageSize must be between 1 and 50." });
        }

        if (bedrooms is < 0)
        {
            return BadRequest(new { error = "bedrooms must be >= 0." });
        }

        if (minPrice is < 0 || maxPrice is < 0)
        {
            return BadRequest(new { error = "minPrice and maxPrice must be >= 0." });
        }

        if (minPrice is not null && maxPrice is not null && minPrice > maxPrice)
        {
            return BadRequest(new { error = "minPrice cannot be greater than maxPrice." });
        }

        var query = _db.Properties.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(city))
        {
            var cityTerm = city.Trim();
            query = query.Where(p => EF.Functions.ILike(p.City, cityTerm));
        }

        if (bedrooms is not null)
        {
            query = query.Where(p => p.Bedrooms == bedrooms);
        }

        if (minPrice is not null)
        {
            query = query.Where(p => p.Price >= minPrice);
        }

        if (maxPrice is not null)
        {
            query = query.Where(p => p.Price <= maxPrice);
        }

        if (hasGarage is not null)
        {
            query = query.Where(p => p.HasGarage == hasGarage);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Price)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PropertyListItemDto(
                p.Id,
                p.AddressLine1,
                p.City,
                p.State,
                p.PostalCode,
                p.Bedrooms,
                p.Bathrooms,
                p.Price,
                p.SquareFeet,
                p.HasGarage))
            .ToListAsync(cancellationToken);

        return Ok(new PagedPropertyResponse(items, page, pageSize, totalCount));
    }
}