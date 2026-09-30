namespace RealEstateAiAgent.Api.Contracts;

public record PropertyEmbeddingBackfillResponse(
    int EmbeddedCount,
    int SkippedEmptyTextCount,
    int AlreadyEmbeddedCount,
    IReadOnlyList<Guid> UpdatedPropertyIds);