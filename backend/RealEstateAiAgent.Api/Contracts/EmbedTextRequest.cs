using System.ComponentModel.DataAnnotations;

namespace RealEstateAiAgent.Api.Contracts;

public class EmbedTextRequest
{
    [Required, MinLength(1), MaxLength(8000)]
    public string Text { get; set; } = string.Empty;
}