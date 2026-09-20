using RealEstateAiAgent.Api.Models;

namespace RealEstateAiAgent.Api.Data;

public static class PropertySeedData
{
    public static IReadOnlyList<Property> GetProperties()
    {
        var created = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

        return new List<Property>
        {
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                AddressLine1 = "1420 Willow Glen Way",
                City = "San Jose",
                State = "CA",
                PostalCode = "95125",
                Bedrooms = 2,
                Bathrooms = 2.0m,
                Price = 1_150_000m,
                SquareFeet = 1180,
                HasGarage = true,
                Description = "Updated condo-style home near Willow Glen with attached two-car garage and open kitchen.",
                CreatedAtUtc = created
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                AddressLine1 = "288 N 4th St Unit 512",
                City = "San Jose",
                State = "CA",
                PostalCode = "95112",
                Bedrooms = 2,
                Bathrooms = 2.0m,
                Price = 899_000m,
                SquareFeet = 980,
                HasGarage = false,
                Description = "Downtown San Jose loft-style unit, walkable to transit, no dedicated garage.",
                CreatedAtUtc = created
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                AddressLine1 = "4567 Almaden Valley Rd",
                City = "San Jose",
                State = "CA",
                PostalCode = "95118",
                Bedrooms = 3,
                Bathrooms = 2.5m,
                Price = 1_450_000m,
                SquareFeet = 1650,
                HasGarage = true,
                Description = "Single-family home in Almaden with large backyard and two-car garage.",
                CreatedAtUtc = created
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111104"),
                AddressLine1 = "890 Castro St",
                City = "Mountain View",
                State = "CA",
                PostalCode = "94041",
                Bedrooms = 2,
                Bathrooms = 1.0m,
                Price = 1_280_000m,
                SquareFeet = 1050,
                HasGarage = true,
                Description = "Charming duplex-style unit near Castro Street shops and dining.",
                CreatedAtUtc = created
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111105"),
                AddressLine1 = "1200 Laurelwood Rd",
                City = "Santa Clara",
                State = "CA",
                PostalCode = "95054",
                Bedrooms = 4,
                Bathrooms = 3.0m,
                Price = 1_750_000m,
                SquareFeet = 2100,
                HasGarage = true,
                Description = "Spacious family home close to tech campuses with remodeled primary suite.",
                CreatedAtUtc = created
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111106"),
                AddressLine1 = "77 Baypointe Pkwy",
                City = "San Jose",
                State = "CA",
                PostalCode = "95134",
                Bedrooms = 2,
                Bathrooms = 2.0m,
                Price = 1_199_000m,
                SquareFeet = 1105,
                HasGarage = true,
                Description = "North San Jose townhome with garage and easy freeway access.",
                CreatedAtUtc = created
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111107"),
                AddressLine1 = "2100 University Ave",
                City = "Palo Alto",
                State = "CA",
                PostalCode = "94301",
                Bedrooms = 2,
                Bathrooms = 1.5m,
                Price = 2_100_000m,
                SquareFeet = 1200,
                HasGarage = false,
                Description = "Classic Palo Alto bungalow-style home, premium location, street parking only.",
                CreatedAtUtc = created
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111108"),
                AddressLine1 = "330 Blossom Hill Rd",
                City = "San Jose",
                State = "CA",
                PostalCode = "95123",
                Bedrooms = 2,
                Bathrooms = 2.5m,
                Price = 975_000m,
                SquareFeet = 1250,
                HasGarage = true,
                Description = "End-unit townhome with garage and low HOA near Blossom Hill.",
                CreatedAtUtc = created
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111109"),
                AddressLine1 = "15 Marina Blvd",
                City = "San Mateo",
                State = "CA",
                PostalCode = "94401",
                Bedrooms = 3,
                Bathrooms = 2.0m,
                Price = 1_320_000m,
                SquareFeet = 1400,
                HasGarage = true,
                Description = "Bay Area peninsula home with garage and short drive to Caltrain.",
                CreatedAtUtc = created
            },
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111110"),
                AddressLine1 = "6021 Snell Ave",
                City = "San Jose",
                State = "CA",
                PostalCode = "95123",
                Bedrooms = 2,
                Bathrooms = 1.0m,
                Price = 825_000m,
                SquareFeet = 900,
                HasGarage = false,
                Description = "Starter home in South San Jose, good value, no garage.",
                CreatedAtUtc = created
            }
        };
    }
}