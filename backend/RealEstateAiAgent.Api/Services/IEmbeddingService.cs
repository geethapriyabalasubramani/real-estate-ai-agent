using Pgvector;

namespace RealEstateAiAgent.Api.Services;

public interface IEmbeddingService
{
    Task<Vector> EmbedAsync(string text, CancellationToken cancellationToken = default);
}