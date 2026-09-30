using Microsoft.EntityFrameworkCore;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Data;
using RealEstateAiAgent.Api.Models;

namespace RealEstateAiAgent.Api.Services;

public class PropertyEmbeddingBackfillService : IPropertyEmbeddingBackfillService
{
    private readonly ApplicationDbContext _db;
    private readonly IEmbeddingService _embeddingService;

    public PropertyEmbeddingBackfillService(
        ApplicationDbContext db,
        IEmbeddingService embeddingService)
    {
        _db = db;
        _embeddingService = embeddingService;
    }

    public async Task<PropertyEmbeddingBackfillResponse> BackfillMissingAsync(
        CancellationToken cancellationToken = default)
    {
        var alreadyEmbeddedCount = await _db.Properties
            .CountAsync(p => p.DescriptionEmbedding != null, cancellationToken);

        var toEmbed = await _db.Properties
            .Where(p => p.DescriptionEmbedding == null)
            .OrderBy(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var embeddedCount = 0;
        var skippedEmptyTextCount = 0;
        var updatedPropertyIds = new List<Guid>();

        foreach (var property in toEmbed)
        {
            var text = BuildEmbeddingText(property);
            if (string.IsNullOrWhiteSpace(text))
            {
                skippedEmptyTextCount++;
                continue;
            }

            property.DescriptionEmbedding = await _embeddingService.EmbedAsync(text, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            embeddedCount++;
            updatedPropertyIds.Add(property.Id);
        }

        return new PropertyEmbeddingBackfillResponse(
            embeddedCount,
            skippedEmptyTextCount,
            alreadyEmbeddedCount,
            updatedPropertyIds);
    }

    private static string BuildEmbeddingText(Property property)
    {
        var garage = property.HasGarage ? "Has garage." : "No garage.";
        return $"{property.City}, {property.State}. {property.Bedrooms} bed, {property.Bathrooms} bath. {garage} {property.Description}";
    }
}
