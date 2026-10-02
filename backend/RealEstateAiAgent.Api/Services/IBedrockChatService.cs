namespace RealEstateAiAgent.Api.Services;

public interface IBedrockChatService
{
    Task<string> CompleteAsync(
        string prompt,
        string? systemPrompt = null,
        int? maxTokens = null,
        double? temperature = null,
        CancellationToken cancellationToken = default);
}
