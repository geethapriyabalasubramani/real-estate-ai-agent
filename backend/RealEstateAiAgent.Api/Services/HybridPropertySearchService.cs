using Amazon.Runtime;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Exceptions;

namespace RealEstateAiAgent.Api.Services;

public class HybridPropertySearchService : IHybridPropertySearchService
{
    private const int RrfK = 60;

    private readonly INaturalLanguagePropertySearchService _naturalLanguageSearch;
    private readonly ISemanticPropertySearchService _semanticSearch;

    public HybridPropertySearchService(
        INaturalLanguagePropertySearchService naturalLanguageSearch,
        ISemanticPropertySearchService semanticSearch)
    {
        _naturalLanguageSearch = naturalLanguageSearch;
        _semanticSearch = semanticSearch;
    }

    public async Task<HybridPropertySearchResponse> SearchAsync(
        HybridPropertySearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = request.Query.Trim();
        var limit = request.Limit;

        var semanticTask = _semanticSearch.SearchAsync(
            new SemanticPropertySearchRequest { Query = query, Limit = limit },
            cancellationToken);
        var filterTask = RunFilterSafeAsync(query, cancellationToken);

        await Task.WhenAll(semanticTask, filterTask);

        var semantic = await semanticTask;
        var (filter, filterNote) = await filterTask;

        var accumulators = new Dictionary<Guid, HybridAccumulator>();

        AddSemanticRanks(semantic.Results, accumulators);
        if (filter is not null)
        {
            AddFilterRanks(filter.Results.Items, accumulators);
        }

        var results = accumulators.Values
            .OrderByDescending(a => a.HybridScore)
            .Take(limit)
            .Select(ToHybridItem)
            .ToList();

        return new HybridPropertySearchResponse(
            query,
            filter?.InterpretedCriteria,
            filterNote,
            results);
    }

    private async Task<(NaturalLanguageSearchResponse? Response, string? Note)> RunFilterSafeAsync(
        string query,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _naturalLanguageSearch.SearchAsync(
                new NaturalLanguageSearchRequest { Query = query },
                cancellationToken);
            return (response, null);
        }
        catch (AiCriteriaValidationException)
        {
            return (null, "Filter search criteria were invalid.");
        }
        catch (AiCriteriaParseException)
        {
            return (null, "Filter search could not interpret the query.");
        }
        catch (AmazonServiceException)
        {
            return (null, "Filter search failed.");
        }
    }

    private static void AddSemanticRanks(
        IReadOnlyList<SemanticPropertySearchResultItem> items,
        Dictionary<Guid, HybridAccumulator> accumulators)
    {
        for (var index = 0; index < items.Count; index++)
        {
            var rank = index + 1;
            var item = items[index];
            var acc = GetOrCreate(accumulators, item.Id);
            acc.HybridScore += 1.0 / (RrfK + rank);
            acc.MatchedSemantic = true;
            ApplySemanticFields(acc, item);
        }
    }

    private static void AddFilterRanks(
        IReadOnlyList<PropertyListItemDto> items,
        Dictionary<Guid, HybridAccumulator> accumulators)
    {
        for (var index = 0; index < items.Count; index++)
        {
            var rank = index + 1;
            var item = items[index];
            var acc = GetOrCreate(accumulators, item.Id);
            acc.HybridScore += 1.0 / (RrfK + rank);
            acc.MatchedFilter = true;
            ApplyListItemFields(acc, item);
        }
    }

    private static HybridAccumulator GetOrCreate(
        Dictionary<Guid, HybridAccumulator> accumulators,
        Guid id)
    {
        if (!accumulators.TryGetValue(id, out var acc))
        {
            acc = new HybridAccumulator { Id = id };
            accumulators[id] = acc;
        }

        return acc;
    }

    private static void ApplyListItemFields(HybridAccumulator acc, PropertyListItemDto item)
    {
        acc.AddressLine1 = item.AddressLine1;
        acc.City = item.City;
        acc.State = item.State;
        acc.PostalCode = item.PostalCode;
        acc.Bedrooms = item.Bedrooms;
        acc.Bathrooms = item.Bathrooms;
        acc.Price = item.Price;
        acc.SquareFeet = item.SquareFeet;
        acc.HasGarage = item.HasGarage;
    }

    private static void ApplySemanticFields(HybridAccumulator acc, SemanticPropertySearchResultItem item)
    {
        acc.AddressLine1 = item.AddressLine1;
        acc.City = item.City;
        acc.State = item.State;
        acc.PostalCode = item.PostalCode;
        acc.Bedrooms = item.Bedrooms;
        acc.Bathrooms = item.Bathrooms;
        acc.Price = item.Price;
        acc.SquareFeet = item.SquareFeet;
        acc.HasGarage = item.HasGarage;
    }

    private static HybridPropertySearchResultItem ToHybridItem(HybridAccumulator acc) =>
        new(
            acc.Id,
            acc.AddressLine1,
            acc.City,
            acc.State,
            acc.PostalCode,
            acc.Bedrooms,
            acc.Bathrooms,
            acc.Price,
            acc.SquareFeet,
            acc.HasGarage,
            acc.HybridScore,
            acc.MatchedSemantic,
            acc.MatchedFilter);

    private sealed class HybridAccumulator
    {
        public Guid Id { get; init; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public decimal Bathrooms { get; set; }
        public decimal Price { get; set; }
        public int SquareFeet { get; set; }
        public bool HasGarage { get; set; }
        public double HybridScore { get; set; }
        public bool MatchedSemantic { get; set; }
        public bool MatchedFilter { get; set; }
    }
}
