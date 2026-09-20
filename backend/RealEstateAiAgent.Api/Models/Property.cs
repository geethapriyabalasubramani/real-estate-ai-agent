namespace RealEstateAiAgent.Api.Models;

public class Property
{
    public Guid Id { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    public int Bedrooms { get; set; }
    public decimal Bathrooms { get; set; }

    public decimal Price { get; set; }
    public int SquareFeet { get; set; }

    public bool HasGarage { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}