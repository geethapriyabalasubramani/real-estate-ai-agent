using System.ComponentModel.DataAnnotations;

namespace RealEstateAiAgent.Api.Contracts;

public class NaturalLanguageSearchRequest
{
    [Required, MinLength(3), MaxLength(500)]
    public string Query { get; set; } = string.Empty;
}