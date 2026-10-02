using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Data;

namespace RealEstateAiAgent.Api.Services;

public class RealEstateAgentToolExecutor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    private readonly IHybridPropertySearchService _hybridSearch;
    private readonly ISemanticPropertySearchService _semanticSearch;
    private readonly IPropertySearchService _propertySearch;
    private readonly IPropertyQuestionAnswerService _propertyQa;
    private readonly ApplicationDbContext _db;

    public RealEstateAgentToolExecutor(
        IHybridPropertySearchService hybridSearch,
        ISemanticPropertySearchService semanticSearch,
        IPropertySearchService propertySearch,
        IPropertyQuestionAnswerService propertyQa,
        ApplicationDbContext db)
    {
        _hybridSearch = hybridSearch;
        _semanticSearch = semanticSearch;
        _propertySearch = propertySearch;
        _propertyQa = propertyQa;
        _db = db;
    }

    public async Task<string> ExecuteAsync(
        string toolName,
        JsonElement arguments,
        CancellationToken cancellationToken)
    {
        return toolName switch
        {
            "hybrid_search" => await HybridSearchAsync(arguments, cancellationToken),
            "semantic_search" => await SemanticSearchAsync(arguments, cancellationToken),
            "filter_search" => await FilterSearchAsync(arguments, cancellationToken),
            "answer_from_catalog" => await AnswerFromCatalogAsync(arguments, cancellationToken),
            "get_property" => await GetPropertyAsync(arguments, cancellationToken),
            _ => JsonSerializer.Serialize(new { error = $"Unknown tool '{toolName}'." }, JsonOptions),
        };
    }

    private async Task<string> HybridSearchAsync(JsonElement args, CancellationToken cancellationToken)
    {
        var query = RequireString(args, "query");
        var limit = ClampLimit(GetOptionalInt(args, "limit") ?? 5);

        var response = await _hybridSearch.SearchAsync(
            new HybridPropertySearchRequest { Query = query, Limit = limit },
            cancellationToken);

        return JsonSerializer.Serialize(new
        {
            query = response.Query,
            interpretedCriteria = response.InterpretedCriteria,
            filterSearchNote = response.FilterSearchNote,
            results = response.Results.Select(r => new
            {
                r.Id,
                r.AddressLine1,
                r.City,
                r.State,
                r.Price,
                r.Bedrooms,
                r.Bathrooms,
                r.HasGarage,
                r.HybridScore,
            }),
        }, JsonOptions);
    }

    private async Task<string> SemanticSearchAsync(JsonElement args, CancellationToken cancellationToken)
    {
        var query = RequireString(args, "query");
        var limit = ClampLimit(GetOptionalInt(args, "limit") ?? 5);

        var response = await _semanticSearch.SearchAsync(
            new SemanticPropertySearchRequest { Query = query, Limit = limit },
            cancellationToken);

        return JsonSerializer.Serialize(new
        {
            query = response.Query,
            results = response.Results.Select(r => new
            {
                r.Id,
                r.AddressLine1,
                r.City,
                r.State,
                r.Price,
                r.Bedrooms,
                r.Bathrooms,
                r.HasGarage,
                r.SimilarityScore,
            }),
        }, JsonOptions);
    }

    private async Task<string> FilterSearchAsync(JsonElement args, CancellationToken cancellationToken)
    {
        var limit = ClampLimit(GetOptionalInt(args, "limit") ?? 10);

        var query = new PropertySearchQuery
        {
            City = GetOptionalString(args, "city"),
            Bedrooms = GetOptionalInt(args, "bedrooms"),
            MinPrice = GetOptionalDecimal(args, "minPrice"),
            MaxPrice = GetOptionalDecimal(args, "maxPrice"),
            HasGarage = GetOptionalBool(args, "hasGarage"),
            Page = 1,
            PageSize = limit,
        };

        PropertySearchQueryValidator.Validate(query);
        var response = await _propertySearch.SearchAsync(query, cancellationToken);

        return JsonSerializer.Serialize(new
        {
            totalCount = response.TotalCount,
            items = response.Items.Select(i => new
            {
                i.Id,
                i.AddressLine1,
                i.City,
                i.State,
                i.Price,
                i.Bedrooms,
                i.Bathrooms,
                i.HasGarage,
            }),
        }, JsonOptions);
    }

    private async Task<string> AnswerFromCatalogAsync(JsonElement args, CancellationToken cancellationToken)
    {
        var question = RequireString(args, "question");
        var topK = ClampLimit(GetOptionalInt(args, "topK") ?? 5);

        var response = await _propertyQa.AskAsync(
            new PropertyAskRequest { Question = question, TopK = topK },
            cancellationToken);

        return JsonSerializer.Serialize(new
        {
            response.Question,
            response.Answer,
            sources = response.Sources.Select(s => new
            {
                s.Id,
                s.AddressLine1,
                s.City,
                s.State,
                s.SimilarityScore,
            }),
        }, JsonOptions);
    }

    private async Task<string> GetPropertyAsync(JsonElement args, CancellationToken cancellationToken)
    {
        var idText = RequireString(args, "id");
        if (!Guid.TryParse(idText, out var id))
        {
            return JsonSerializer.Serialize(new { error = "Invalid id; expected a GUID." }, JsonOptions);
        }

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
            return JsonSerializer.Serialize(new { error = "Property not found.", id }, JsonOptions);
        }

        return JsonSerializer.Serialize(property, JsonOptions);
    }

    private static string RequireString(JsonElement args, string name)
    {
        if (!args.TryGetProperty(name, out var prop) || prop.ValueKind != JsonValueKind.String)
        {
            throw new ArgumentException($"Tool argument '{name}' is required.");
        }

        var value = prop.GetString()?.Trim();
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Tool argument '{name}' is required.");
        }

        return value;
    }

    private static string? GetOptionalString(JsonElement args, string name)
    {
        if (!args.TryGetProperty(name, out var prop) || prop.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return prop.GetString()?.Trim();
    }

    private static int? GetOptionalInt(JsonElement args, string name)
    {
        if (!args.TryGetProperty(name, out var prop) || prop.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return prop.TryGetInt32(out var value) ? value : null;
    }

    private static decimal? GetOptionalDecimal(JsonElement args, string name)
    {
        if (!args.TryGetProperty(name, out var prop) || prop.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return prop.TryGetDecimal(out var value) ? value : null;
    }

    private static bool? GetOptionalBool(JsonElement args, string name)
    {
        if (!args.TryGetProperty(name, out var prop) || prop.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return prop.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => null,
        };
    }

    private static int ClampLimit(int limit) => Math.Clamp(limit, 1, 20);
}
