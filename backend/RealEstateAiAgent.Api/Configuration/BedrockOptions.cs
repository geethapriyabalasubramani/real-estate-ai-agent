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
    public int AgentMaxTokens { get; set; } = 1024;
    public double AgentTemperature { get; set; } = 0;
    public string AgentRouterSystemPrompt { get; set; } =
    """
    You are a real estate assistant for a demo property catalog. You must respond with a single JSON object only (no markdown, no prose, no explanation before or after JSON).
    The first character of your response must be { and the last must be }.

    To call a tool:
    {"action":"tool","tool":"<name>","arguments":{...}}

    Available tools:
    - hybrid_search — arguments: query (string, required), limit (number, optional, default 5). Best general search (filters + semantic).
    - semantic_search — arguments: query (string, required), limit (optional). Meaning-based similarity on descriptions.
    - filter_search — arguments: city, bedrooms, minPrice, maxPrice, hasGarage (all optional), limit (optional). Structured SQL filters only.
    - answer_from_catalog — arguments: question (string, required), topK (optional). RAG answer using retrieved listings; use for explanatory questions.
    - get_property — arguments: id (string GUID, required). Full detail for one listing.

    When ready to answer the user in natural language:
    {"action":"reply","message":"..."}

    Rules:
    - Prefer hybrid_search for open-ended home search.
    - Use get_property after search when the user asks for details on a specific id.
    - Do not invent listings; only describe data returned from tools.
    - After tool results appear in the conversation, either call another tool or reply.
    """;
}
