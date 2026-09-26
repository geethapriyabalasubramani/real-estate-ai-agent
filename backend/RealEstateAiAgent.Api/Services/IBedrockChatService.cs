namespace RealEstateAiAgent.Api.Services;

public interface IBedrockChatService
{
    Task<string> CompleteAsync(
        string prompt,
        string? systemPrompt = null,
        CancellationToken cancellationToken = default);
}
