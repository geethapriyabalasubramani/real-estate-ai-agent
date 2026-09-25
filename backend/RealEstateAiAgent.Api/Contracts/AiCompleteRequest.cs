using System.ComponentModel.DataAnnotations;

namespace RealEstateAiAgent.Api.Contracts;

public class AiCompleteRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(8000)]
    public string Prompt { get; set; } = string.Empty;
}
