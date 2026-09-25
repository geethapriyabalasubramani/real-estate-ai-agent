using Microsoft.EntityFrameworkCore;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Data;

namespace RealEstateAiAgent.Api.Services;

public class PropertySearchService : IPropertySearchService
{
    private readonly ApplicationDbContext _db;

    public PropertySearchService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedPropertyResponse> SearchAsync(
        PropertySearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var dbQuery = _db.Properties.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.City))
        {
            var cityTerm = query.City.Trim();
            dbQuery = dbQuery.Where(p => EF.Functions.ILike(p.City, cityTerm));
        }

        if (query.Bedrooms is not null)
        {
            dbQuery = dbQuery.Where(p => p.Bedrooms == query.Bedrooms);
        }

        if (query.MinPrice is not null)
        {
            dbQuery = dbQuery.Where(p => p.Price >= query.MinPrice);
        }

        if (query.MaxPrice is not null)
        {
            dbQuery = dbQuery.Where(p => p.Price <= query.MaxPrice);
        }

        if (query.HasGarage is not null)
        {
            dbQuery = dbQuery.Where(p => p.HasGarage == query.HasGarage);
        }

        var totalCount = await dbQuery.CountAsync(cancellationToken);

        var items = await dbQuery
            .OrderBy(p => p.Price)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
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

        return new PagedPropertyResponse(items, query.Page, query.PageSize, totalCount);
    }
}
