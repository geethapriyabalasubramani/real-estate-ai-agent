namespace RealEstateAiAgent.Api.Contracts;

public record PropertyListItemDto(
    Guid Id,
    string AddressLine1,
    string City,
    string State,
    string PostalCode,
    int Bedrooms,
    decimal Bathrooms,
    decimal Price,
    int SquareFeet,
    bool HasGarage
);