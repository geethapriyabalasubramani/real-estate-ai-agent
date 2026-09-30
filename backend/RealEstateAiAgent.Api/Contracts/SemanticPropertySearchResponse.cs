namespace RealEstateAiAgent.Api.Contracts;

public record SemanticPropertySearchResponse(
    string Query,
    int Limit,
    IReadOnlyList<SemanticPropertySearchResultItem> Results);