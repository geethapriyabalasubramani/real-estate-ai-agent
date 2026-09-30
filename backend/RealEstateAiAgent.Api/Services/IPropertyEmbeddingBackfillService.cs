using RealEstateAiAgent.Api.Contracts;

namespace RealEstateAiAgent.Api.Services;

public interface IPropertyEmbeddingBackfillService
{
    /// <summary>
    /// Embeds properties missing DescriptionEmbedding and saves to the database.
    /// </summary>
    Task<PropertyEmbeddingBackfillResponse> BackfillMissingAsync(
        CancellationToken cancellationToken = default);
}