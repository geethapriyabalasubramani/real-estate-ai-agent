using Amazon;
using Amazon.BedrockRuntime;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Amazon;
using RealEstateAiAgent.Api.Configuration;

namespace RealEstateAiAgent.Api.Services;

public class BedrockChatService : IBedrockChatService
{
    private readonly Kernel _kernel;
    private readonly BedrockOptions _options;

    public BedrockChatService(Kernel kernel, IOptions<BedrockOptions> options)
    {
        _kernel = kernel;
        _options = options.Value;
    }

    public async Task<string> CompleteAsync(
        string prompt,
        string? systemPrompt = null,
        int? maxTokens = null,
        double? temperature = null,
        CancellationToken cancellationToken = default)
    {
        var chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();
        history.AddSystemMessage(systemPrompt ?? _options.SystemPrompt);
        history.AddUserMessage(prompt);

        var tokenLimit = maxTokens ?? _options.MaxTokens;
        tokenLimit = tokenLimit >= 1 ? tokenLimit : 256;
        var temp = temperature ?? _options.Temperature;

        var settings = new AmazonClaudeExecutionSettings
        {
            MaxTokensToSample = tokenLimit,
            Temperature = (float)temp,
        };

        var result = await chatCompletion.GetChatMessageContentsAsync(
            history,
            settings,
            _kernel,
            cancellationToken);

        var text = result[^1].Content;

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("Semantic Kernel returned an empty response.");
        }

        return text.Trim();
    }
}


public static class BedrockServiceCollectionExtensions
{
    public static IServiceCollection AddBedrockAi(this IServiceCollection services, IConfiguration configuration)
    {
        var bedrockSection = configuration.GetSection(BedrockOptions.SectionName);
        services.Configure<BedrockOptions>(bedrockSection);

        var bedrockConfig = bedrockSection.Get<BedrockOptions>()
            ?? throw new InvalidOperationException("Bedrock configuration is missing.");

        if (string.IsNullOrWhiteSpace(bedrockConfig.ModelId))
        {
            throw new InvalidOperationException("Bedrock:ModelId is not configured.");
        }

        var region = RegionEndpoint.GetBySystemName(bedrockConfig.Region);

        services.AddSingleton<IAmazonBedrockRuntime>(_ => new AmazonBedrockRuntimeClient(region));

        services
            .AddKernel()
            .AddBedrockChatCompletionService(
                modelId: bedrockConfig.ModelId,
                bedrockRuntime: null,
                serviceId: null);

        services.AddScoped<IBedrockChatService, BedrockChatService>();
        services.AddScoped<IEmbeddingService, BedrockEmbeddingService>();

        return services;
    }
}
