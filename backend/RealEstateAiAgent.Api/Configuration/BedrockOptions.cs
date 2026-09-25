namespace RealEstateAiAgent.Api.Configuration;

public class BedrockOptions
{
    public const string SectionName = "Bedrock";

    public string Region { get; set; } = "us-east-1";
    public string ModelId { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 256;
    public double Temperature { get; set; } = 0.2;
    public string SystemPrompt { get; set; } =
    "You are a helpful real estate assistant. Answer clearly and concisely.";
}
