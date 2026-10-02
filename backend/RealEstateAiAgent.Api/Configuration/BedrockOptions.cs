namespace RealEstateAiAgent.Api.Configuration;

public class BedrockOptions
{
    public const string SectionName = "Bedrock";

    public string Region { get; set; } = "us-east-1";
    public string ModelId { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 256;
    public double Temperature { get; set; } = 0.2;
    public string EmbeddingModelId { get; set; } = "amazon.titan-embed-text-v2:0";

/// <summary>Must match DB column vector(N) and Titan v2 dimensions.</summary>
public int EmbeddingDimensions { get; set; } = 1024;
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
    public string RagSystemPrompt { get; set; } =
    """
    You are a real estate assistant for a demo property catalog.
    You will receive CONTEXT: numbered listing snippets from the database.
    Rules:
    - Answer the user's QUESTION using ONLY information in CONTEXT.
    - If CONTEXT is insufficient, say you don't have enough information in the catalog.
    - Do not invent properties, prices, or features not in CONTEXT.
    - Be concise (2-5 sentences unless the user asks for detail).
    - When mentioning a listing, include its city and street address from CONTEXT.
    """;
}
