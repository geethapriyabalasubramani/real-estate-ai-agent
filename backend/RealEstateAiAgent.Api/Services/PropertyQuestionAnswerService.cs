using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pgvector;
using RealEstateAiAgent.Api.Configuration;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Data;
using RealEstateAiAgent.Api.Models;

namespace RealEstateAiAgent.Api.Services;

public class PropertyQuestionAnswerService : IPropertyQuestionAnswerService
{
    private readonly ApplicationDbContext _db;
    private readonly IEmbeddingService _embeddingService;
    private readonly IBedrockChatService _bedrockChatService;
    private readonly BedrockOptions _options;

    public PropertyQuestionAnswerService(
        ApplicationDbContext db,
        IEmbeddingService embeddingService,
        IBedrockChatService bedrockChatService,
        IOptions<BedrockOptions> bedrockOptions)
    {
        _db = db;
        _embeddingService = embeddingService;
        _bedrockChatService = bedrockChatService;
        _options = bedrockOptions.Value;
    }

    public async Task<PropertyAskResponse> AskAsync(
        PropertyAskRequest request,
        CancellationToken cancellationToken = default)
    {
        var question = request.Question.Trim();
        var topK = request.TopK;

        var queryVector = await _embeddingService.EmbedAsync(question, cancellationToken);

        var candidates = await _db.Properties
            .FromSqlInterpolated($"""
                SELECT p."Id", p."AddressLine1", p."City", p."State", p."PostalCode",
                       p."Bedrooms", p."Bathrooms", p."Price", p."SquareFeet", p."HasGarage",
                       p."Description", p."CreatedAtUtc", p."DescriptionEmbedding"
                FROM "Properties" AS p
                WHERE p."DescriptionEmbedding" IS NOT NULL
                ORDER BY p."DescriptionEmbedding" <=> {queryVector}
                LIMIT {topK}
                """)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var sources = candidates
            .Select(p =>
            {
                var score = 1.0 - CosineDistanceNormalized(p.DescriptionEmbedding!, queryVector);
                return new PropertyAskSourceItem(
                    p.Id,
                    p.AddressLine1,
                    p.City,
                    p.State,
                    score);
            })
            .ToList();

        var context = BuildContext(candidates);
        var userPrompt =
            $"""
            CONTEXT:
            {context}

            QUESTION:
            {question}
            """;

        var answer = await _bedrockChatService.CompleteAsync(
            userPrompt,
            _options.RagSystemPrompt,
            cancellationToken);

        return new PropertyAskResponse(question, answer, sources);
    }

    private static string BuildContext(IReadOnlyList<Property> properties)
    {
        if (properties.Count == 0)
        {
            return "(No listings with embeddings in the catalog.)";
        }

        var builder = new StringBuilder();
        for (var i = 0; i < properties.Count; i++)
        {
            var p = properties[i];
            var garage = p.HasGarage ? "Has garage." : "No garage.";
            builder.AppendLine(
                $"{i + 1}. {p.AddressLine1}, {p.City}, {p.State} {p.PostalCode} — " +
                $"{p.Bedrooms} bed, {p.Bathrooms} bath, ${p.Price:N0}, {p.SquareFeet} sq ft. {garage} {p.Description}");
        }

        return builder.ToString().TrimEnd();
    }

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
