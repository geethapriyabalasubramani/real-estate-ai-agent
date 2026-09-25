using System.Text.Json;
using RealEstateAiAgent.Api.Contracts;

namespace RealEstateAiAgent.Api.Services;

public class NaturalLanguagePropertySearchService : INaturalLanguagePropertySearchService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IBedrockChatService _bedrockChatService;
    private readonly IPropertySearchService _propertySearch;

    public NaturalLanguagePropertySearchService(
        IBedrockChatService bedrockChatService,
        IPropertySearchService propertySearch)
    {
        _bedrockChatService = bedrockChatService;
        _propertySearch = propertySearch;
    }

    public async Task<NaturalLanguageSearchResponse> SearchAsync(
        NaturalLanguageSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var prompt =
            """
            Extract property search filters from the user message below.
            Reply with JSON only (no markdown), using this shape:
            {"city": string or null, "bedrooms": number or null, "minPrice": number or null, "maxPrice": number or null, "hasGarage": true/false or null}

            User message:
            """
            + request.Query.Trim();

        var raw = await _bedrockChatService.CompleteAsync(prompt, cancellationToken);
        var criteria = ParseCriteria(raw);

        var query = new PropertySearchQuery
        {
            City = criteria.City,
            Bedrooms = criteria.Bedrooms,
            MinPrice = criteria.MinPrice,
            MaxPrice = criteria.MaxPrice,
            HasGarage = criteria.HasGarage,
            Page = 1,
            PageSize = 10,
        };

        var results = await _propertySearch.SearchAsync(query, cancellationToken);
        return new NaturalLanguageSearchResponse(criteria, results);
    }

    private static AiPropertySearchCriteria ParseCriteria(string raw)
    {
        var json = ExtractJsonObject(raw);
        try
        {
            return JsonSerializer.Deserialize<AiPropertySearchCriteria>(json, JsonOptions)
                ?? throw new InvalidOperationException("Could not interpret the search query.");
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Could not interpret the search query.");
        }
    }

    private static string ExtractJsonObject(string raw)
    {
        var trimmed = raw.Trim();
        if (trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline >= 0)
            {
                trimmed = trimmed[(firstNewline + 1)..];
            }

            var fence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
            if (fence >= 0)
            {
                trimmed = trimmed[..fence];
            }

            trimmed = trimmed.Trim();
        }

        var start = trimmed.IndexOf('{');
        var end = trimmed.LastIndexOf('}');
        if (start < 0 || end <= start)
        {
            throw new InvalidOperationException("Could not interpret the search query.");
        }

        return trimmed[start..(end + 1)];
    }
}
