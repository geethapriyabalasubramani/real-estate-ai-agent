using System.ComponentModel.DataAnnotations;

namespace RealEstateAiAgent.Api.Contracts;

public class SemanticPropertySearchRequest
{
    [Required, MinLength(3), MaxLength(500)]
    public string Query { get; set; } = string.Empty;

    [Range(1, 20)]
    public int Limit { get; set; } = 5;
}