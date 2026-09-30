using System.ComponentModel.DataAnnotations;

namespace RealEstateAiAgent.Api.Contracts;

public class HybridPropertySearchRequest
{
    [Required, MinLength(3), MaxLength(500)]
    public string Query { get; set; } = string.Empty;

    [Range(1, 20)]
    public int Limit { get; set; } = 10;
}