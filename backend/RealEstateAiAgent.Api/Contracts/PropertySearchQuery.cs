using System.ComponentModel.DataAnnotations;

namespace RealEstateAiAgent.Api.Contracts;

public class PropertySearchQuery : IValidatableObject
{
    [MaxLength(100, ErrorMessage = "city must be at most 100 characters.")]
    public string? City { get; set; }

    [Range(0, 20, ErrorMessage = "bedrooms must be between 0 and 20.")]
    public int? Bedrooms { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "minPrice must be >= 0.")]
    public decimal? MinPrice { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "maxPrice must be >= 0.")]
    public decimal? MaxPrice { get; set; }

    public bool? HasGarage { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "page must be >= 1.")]
    public int Page { get; set; } = 1;

    [Range(1, 50, ErrorMessage = "pageSize must be between 1 and 50.")]
    public int PageSize { get; set; } = 10;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinPrice is not null && MaxPrice is not null && MinPrice > MaxPrice)
        {
            yield return new ValidationResult(
                "minPrice cannot be greater than maxPrice.",
                [nameof(MinPrice), nameof(MaxPrice)]);
        }
    }
}