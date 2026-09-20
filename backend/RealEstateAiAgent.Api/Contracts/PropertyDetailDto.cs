namespace RealEstateAiAgent.Api.Contracts;

public record PropertyDetailDto(
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
    string Description,
    DateTime CreatedAtUtc
);