namespace RealEstateAiAgent.Api.Contracts;

public record EmbedTextResponse(
    string ModelId,
    int Dimensions,
    IReadOnlyList<float> Preview);