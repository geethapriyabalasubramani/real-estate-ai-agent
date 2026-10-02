namespace RealEstateAiAgent.Api.Contracts;

public record PropertyAskSourceItem(
    Guid Id,
    string AddressLine1,
    string City,
    string State,
    double SimilarityScore);