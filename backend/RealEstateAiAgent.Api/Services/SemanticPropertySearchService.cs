using Microsoft.EntityFrameworkCore;
using Pgvector;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Data;

namespace RealEstateAiAgent.Api.Services;

public class SemanticPropertySearchService : ISemanticPropertySearchService
{
    private readonly ApplicationDbContext _db;
    private readonly IEmbeddingService _embeddingService;

    public SemanticPropertySearchService(
        ApplicationDbContext db,
        IEmbeddingService embeddingService)
    {
        _db = db;
        _embeddingService = embeddingService;
    }

    public async Task<SemanticPropertySearchResponse> SearchAsync(
        SemanticPropertySearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var trimmedQuery = request.Query.Trim();
        var queryVector = await _embeddingService.EmbedAsync(trimmedQuery, cancellationToken);

        var limit = request.Limit;

        // LINQ CosineDistance can fall back to client evaluation with some EF/Npgsql versions;
        // ORDER BY <=> in SQL keeps nearest-neighbor search on Postgres.
        var candidates = await _db.Properties
            .FromSqlInterpolated($"""
                SELECT p."Id", p."AddressLine1", p."City", p."State", p."PostalCode",
                       p."Bedrooms", p."Bathrooms", p."Price", p."SquareFeet", p."HasGarage",
                       p."Description", p."CreatedAtUtc", p."DescriptionEmbedding"
                FROM "Properties" AS p
                WHERE p."DescriptionEmbedding" IS NOT NULL
                ORDER BY p."DescriptionEmbedding" <=> {queryVector}
                LIMIT {limit}
                """)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var results = candidates
            .Select(p =>
            {
                var distance = CosineDistanceNormalized(p.DescriptionEmbedding!, queryVector);
                return new SemanticPropertySearchResultItem(
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
                    1.0 - distance);
            })
            .ToList();

        return new SemanticPropertySearchResponse(trimmedQuery, limit, results);
    }

    /// <summary>
    /// Cosine distance for L2-normalized vectors (Titan embed uses normalize: true).
    /// </summary>
    private static double CosineDistanceNormalized(Vector stored, Vector query)
    {
        var a = stored.ToArray();
        var b = query.ToArray();
        if (a.Length != b.Length)
        {
            throw new InvalidOperationException("Embedding dimension mismatch.");
        }

        double dot = 0;
        for (var i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
        }

        return 1.0 - dot;
    }
}
