namespace RealEstateAiAgent.Api.Contracts;

public record SemanticPropertySearchResultItem(
    Guid Id,
    string AddressLine1,
    string City,
    string State,
    string PostalCode,
    int Bedrooms,
    decimal Bathrooms,
    decimal Price,
    int SquareFeet,
    bool HasGarage,
    double SimilarityScore);