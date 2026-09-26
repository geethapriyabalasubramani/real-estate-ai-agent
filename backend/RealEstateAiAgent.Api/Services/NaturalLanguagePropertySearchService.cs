using System.Text.Json;
using Microsoft.Extensions.Options;
using RealEstateAiAgent.Api.Configuration;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Exceptions;

namespace RealEstateAiAgent.Api.Services;

public class NaturalLanguagePropertySearchService : INaturalLanguagePropertySearchService
{
    private const string CriteriaParseError = "Could not interpret the search query.";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IBedrockChatService _bedrockChatService;
    private readonly IPropertySearchService _propertySearch;
    private readonly BedrockOptions _options;

    public NaturalLanguagePropertySearchService(
        IBedrockChatService bedrockChatService,
        IPropertySearchService propertySearch,
        IOptions<BedrockOptions> bedrockOptions)
    {
        _bedrockChatService = bedrockChatService;
        _propertySearch = propertySearch;
        _options = bedrockOptions.Value;
    }

    public async Task<NaturalLanguageSearchResponse> SearchAsync(
        NaturalLanguageSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var userPrompt = "User message:\n" + request.Query.Trim();
        var raw = await _bedrockChatService.CompleteAsync(
            userPrompt,
            _options.ExtractionSystemPrompt,
            cancellationToken);
        var criteria = ParseCriteria(raw);

        var query = MapToQuery(criteria);
        PropertySearchQueryValidator.Validate(query);

        var results = await _propertySearch.SearchAsync(query, cancellationToken);
        return new NaturalLanguageSearchResponse(criteria, results);
    }

    private static PropertySearchQuery MapToQuery(AiPropertySearchCriteria criteria) =>
        new()
        {
            City = criteria.City,
            Bedrooms = criteria.Bedrooms,
            MinPrice = criteria.MinPrice,
            MaxPrice = criteria.MaxPrice,
            HasGarage = criteria.HasGarage,
            Page = 1,
            PageSize = 10,
        };

    private static AiPropertySearchCriteria ParseCriteria(string raw)
    {
        var json = ExtractJsonObject(raw);
        try
        {
            return JsonSerializer.Deserialize<AiPropertySearchCriteria>(json, JsonOptions)
                ?? throw new AiCriteriaParseException(CriteriaParseError);
        }
        catch (JsonException)
        {
            throw new AiCriteriaParseException(CriteriaParseError);
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
            throw new AiCriteriaParseException(CriteriaParseError);
        }

        return trimmed[start..(end + 1)];
    }
}
