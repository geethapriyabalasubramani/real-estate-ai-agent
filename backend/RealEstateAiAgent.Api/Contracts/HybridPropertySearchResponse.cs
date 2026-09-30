namespace RealEstateAiAgent.Api.Contracts;

public record HybridPropertySearchResponse(
    string Query,
    AiPropertySearchCriteria? InterpretedCriteria,
    string? FilterSearchNote,
    IReadOnlyList<HybridPropertySearchResultItem> Results);