using System.Text;
using System.Text.Json;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Microsoft.Extensions.Options;
using Pgvector;
using RealEstateAiAgent.Api.Configuration;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace RealEstateAiAgent.Api.Services;

public class BedrockEmbeddingService : IEmbeddingService
{
    private readonly IAmazonBedrockRuntime _client;
    private readonly BedrockOptions _options;

    public BedrockEmbeddingService(IAmazonBedrockRuntime client, IOptions<BedrockOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<Vector> EmbedAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text is required.", nameof(text));
        }

        var body = JsonSerializer.Serialize(new
        {
            inputText = text.Trim(),
            dimensions = _options.EmbeddingDimensions,
            normalize = true,
        });

        var request = new InvokeModelRequest
        {
            ModelId = _options.EmbeddingModelId,
            ContentType = "application/json",
            Accept = "application/json",
            Body = new MemoryStream(Encoding.UTF8.GetBytes(body)),
        };

        var response = await _client.InvokeModelAsync(request, cancellationToken);

        using var reader = new StreamReader(response.Body);
        var json = await reader.ReadToEndAsync(cancellationToken);

        using var doc = JsonDocument.Parse(json);
        var embedding = doc.RootElement.GetProperty("embedding");
        var floats = new float[embedding.GetArrayLength()];

        var i = 0;
        foreach (var value in embedding.EnumerateArray())
        {
            floats[i++] = (float)value.GetDouble();
        }

        if (floats.Length != _options.EmbeddingDimensions)
        {
            throw new InvalidOperationException(
                $"Expected {_options.EmbeddingDimensions} dimensions, got {floats.Length}.");
        }

        return new Vector(floats);
    }
}