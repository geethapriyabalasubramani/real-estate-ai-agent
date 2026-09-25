namespace RealEstateAiAgent.Api.Contracts;

public record NaturalLanguageSearchResponse(
    AiPropertySearchCriteria InterpretedCriteria,
    PagedPropertyResponse Results
);