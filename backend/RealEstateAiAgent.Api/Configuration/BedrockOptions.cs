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
    public string ExtractionSystemPrompt { get; set; } =
    """
    You extract structured property search filters from user messages.
    Reply with a single JSON object only. No markdown, no prose.
    Use null for unknown fields.
    Keys: city (string), bedrooms (number), minPrice (number), maxPrice (number), hasGarage (boolean).
    Prices are USD without commas.
    """;
}
