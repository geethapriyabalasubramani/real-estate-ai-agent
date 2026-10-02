using System.ComponentModel.DataAnnotations;

namespace RealEstateAiAgent.Api.Contracts;

public class AgentChatRequest
{
    [Required, MinLength(1), MaxLength(2000)]
    public string Message { get; set; } = string.Empty;
}
