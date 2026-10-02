using System.ComponentModel.DataAnnotations;

namespace RealEstateAiAgent.Api.Contracts;

public class PropertyAskRequest
{
    [Required, MinLength(3), MaxLength(500)]
    public string Question { get; set; } = string.Empty;

    [Range(1, 10)]
    public int TopK { get; set; } = 5;
}